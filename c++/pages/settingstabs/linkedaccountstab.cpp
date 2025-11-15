#include "linkedaccountstab.h"
#include "config.h"
#include "helper/glowavatar.h"
#include "widgets/settingscard.h"
#include "helper/thememanager.h"
#include "themes/theme.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QNetworkAccessManager>
#include <QNetworkRequest>
#include <QNetworkReply>
#include <QPixmap>
#include <QPalette>
#include <QLabel>
#include <QSizePolicy>
#include <QLoggingCategory>
#include <algorithm>
#include <QProcessEnvironment>

#include "helper/facebookdebug.h"

Q_LOGGING_CATEGORY(linkedAccountsLog, "ui.settings.linkedAccounts")

namespace {

QString resolveRedirectUri(const char *envVarName, const QString &fallback)
{
    const QString fromEnv = qEnvironmentVariable(envVarName);
    const QString trimmed = fromEnv.trimmed();

    if (!trimmed.isEmpty()) {
        return trimmed;
    }

    return fallback;
}

} // namespace

LinkedAccountsTab::LinkedAccountsTab(FacebookClient* client, QWidget *parent)
    : ThemeableWidget(parent), m_facebookClient(client)
{
    m_googleCalendarClient = new GoogleCalendarClient("465071610963-tlaecbljc5ufq3sc3059kin61smmj0pe.apps.googleusercontent.com", "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", this);
    m_avatarNetworkManager = new QNetworkAccessManager(this);
    connect(m_avatarNetworkManager, &QNetworkAccessManager::finished, this, &LinkedAccountsTab::onAvatarDownloaded);

    auto *mainLayout = new QVBoxLayout(this);
    mainLayout->setContentsMargins(24, 24, 24, 24);
    mainLayout->setSpacing(24);

    // Google Account Section
    m_googleCard = new SettingsCard(this);
    auto *googleLayout = m_googleCard->contentLayout();
    googleLayout->addWidget(createSectionHeader(":/icons/calendar.png", tr("Google Calendar")));
    googleLayout->addWidget(createCaptionLabel(tr("Sync appointments by connecting your Google account.")));

    m_linkGoogleAccountButton = new QPushButton(tr("Link Google Account"), m_googleCard);
    m_googleAccountStatusLabel = createCaptionLabel(tr("Not linked"));
    m_googleAccountStatusLabel->setObjectName("googleStatusLabel");

    auto *googleButtons = new QHBoxLayout;
    googleButtons->setContentsMargins(0, 0, 0, 0);
    googleButtons->setSpacing(12);
    googleButtons->addWidget(m_linkGoogleAccountButton);
    googleButtons->addStretch();
    googleButtons->addWidget(m_googleAccountStatusLabel);
    googleLayout->addLayout(googleButtons);

    // Facebook Account Section
    m_facebookCard = new SettingsCard(this);
    auto *facebookLayout = m_facebookCard->contentLayout();
    facebookLayout->addWidget(createSectionHeader(":/icons/menu.png", tr("Facebook Pages")));
    facebookLayout->addWidget(createCaptionLabel(tr("Publish and manage posts by linking your Facebook profile.")));

    setupFacebookUI(facebookLayout);
    m_facebookProfileCard->hide();

    mainLayout->addWidget(m_googleCard);
    mainLayout->addWidget(m_facebookCard);
    mainLayout->addStretch();

    connect(m_linkGoogleAccountButton, &QPushButton::clicked, this, &LinkedAccountsTab::linkGoogleAccount);
    connect(m_linkFacebookAccountButton, &QPushButton::clicked, this, &LinkedAccountsTab::linkFacebookAccount);
    connect(m_facebookClient, &FacebookClient::authenticated, this, &LinkedAccountsTab::onFacebookAuthenticationFinished);
    connect(m_facebookClient, &FacebookClient::authenticationFailed, this, &LinkedAccountsTab::onFacebookAuthenticationFailed);
    connect(m_facebookClient, &FacebookClient::userProfileFetched, this, &LinkedAccountsTab::onFacebookProfileFetched);
    connect(m_facebookClient, &FacebookClient::pagesFetched, this, &LinkedAccountsTab::onFacebookPagesFetched);

    updateTheme();
}

void LinkedAccountsTab::setupFacebookUI(QVBoxLayout *contentLayout) {
    m_linkFacebookAccountButton = new QPushButton(tr("Link Facebook Account"), m_facebookCard);

    m_facebookProfileCard = new QWidget(m_facebookCard);
    auto *profileLayout = new QHBoxLayout(m_facebookProfileCard);
    profileLayout->setContentsMargins(0, 0, 0, 0);
    profileLayout->setSpacing(12);

    m_facebookAvatar = new GlowAvatar(m_facebookProfileCard);
    m_facebookNameLabel = new QLabel(m_facebookProfileCard);
    m_facebookPagesComboBox = new QComboBox(m_facebookProfileCard);

    QLabel *pageLabel = new QLabel(tr("Page"), m_facebookProfileCard);
    pageLabel->setObjectName("facebookPageLabel");

    profileLayout->addWidget(m_facebookAvatar);
    profileLayout->addWidget(m_facebookNameLabel);
    profileLayout->addStretch();
    profileLayout->addWidget(pageLabel);
    profileLayout->addWidget(m_facebookPagesComboBox);

    contentLayout->addWidget(m_linkFacebookAccountButton);
    contentLayout->addWidget(m_facebookProfileCard);

    connect(m_facebookPagesComboBox, QOverload<int>::of(&QComboBox::currentIndexChanged), this, [=](int index){
        if (index >= 0 && index < m_facebookPages.size()) {
            emit facebookPageSelected(m_facebookPages[index]);
        }
    });
}

void LinkedAccountsTab::linkGoogleAccount() {
    const QString googleRedirect = resolveRedirectUri("GOOGLE_REDIRECT_URI", QStringLiteral("http://localhost:8080"));
    m_googleCalendarClient->setRedirectUri(googleRedirect);
    m_googleCalendarClient->authenticate();
    // TODO: Update status label based on authentication result
}

void LinkedAccountsTab::linkFacebookAccount() {
    if (isFacebookDebugLoggingEnabled(linkedAccountsLog())) {
        qCDebug(linkedAccountsLog) << "Link Facebook Account button clicked";
    }
    const QString facebookRedirect =
        resolveRedirectUri("FACEBOOK_REDIRECT_URI",
                           QStringLiteral("https://tattoo-shop-manager.vercel.app/auth/facebook/callback"));
    if (isFacebookDebugLoggingEnabled(linkedAccountsLog())) {
        qCDebug(linkedAccountsLog) << "Using Facebook redirect URI" << facebookRedirect;
    }
    m_facebookClient->setRedirectUri(facebookRedirect);
    m_facebookClient->authenticate();
}

void LinkedAccountsTab::onFacebookAuthenticationFinished(const QString& accessToken) {
    if (isFacebookDebugLoggingEnabled(linkedAccountsLog())) {
        qCDebug(linkedAccountsLog) << "Facebook authentication finished with token"
                                   << (accessToken.isEmpty() ? QStringLiteral("<empty>") : accessToken.left(6) + QStringLiteral("…"));
    }
    m_linkFacebookAccountButton->hide();
    m_facebookProfileCard->show();
    m_facebookClient->fetchUserProfile();
    m_facebookClient->fetchPages();
    if (isFacebookDebugLoggingEnabled(linkedAccountsLog())) {
        qCDebug(linkedAccountsLog) << "Emitting facebookAccountLinked(true)";
    }
    emit facebookAccountLinked(true);
}

void LinkedAccountsTab::onFacebookAuthenticationFailed(const QString& error) {
    if (isFacebookDebugLoggingEnabled(linkedAccountsLog())) {
        qCDebug(linkedAccountsLog) << "Facebook authentication failed with error:" << error;
        qCDebug(linkedAccountsLog) << "Emitting facebookAccountLinked(false)";
    }
    // Show an error message to the user
    emit facebookAccountLinked(false);
}

void LinkedAccountsTab::onFacebookProfileFetched(const FacebookProfile& profile) {
    if (isFacebookDebugLoggingEnabled(linkedAccountsLog())) {
        qCDebug(linkedAccountsLog) << "Facebook profile fetched" << profile.name << "(" << profile.id << ")";
    }
    m_facebookNameLabel->setText(profile.name);

    if (!profile.pictureUrl.isValid()) {
        qWarning() << "Invalid Facebook profile picture URL received.";
        return;
    }

    if (!m_avatarNetworkManager) {
        m_avatarNetworkManager = new QNetworkAccessManager(this);
        connect(m_avatarNetworkManager, &QNetworkAccessManager::finished, this, &LinkedAccountsTab::onAvatarDownloaded);
    }

    m_avatarNetworkManager->get(QNetworkRequest(profile.pictureUrl));
}

void LinkedAccountsTab::onAvatarDownloaded(QNetworkReply* reply) {
    if (!reply) {
        return;
    }

    if (reply->error() == QNetworkReply::NoError) {
        const QByteArray data = reply->readAll();
        if (!data.isEmpty()) {
            QPixmap pixmap;
            if (pixmap.loadFromData(data)) {
                m_facebookAvatar->setPixmap(pixmap);
            } else {
                qWarning() << "Failed to decode Facebook avatar image.";
            }
        } else {
            qWarning() << "Received empty Facebook avatar image response.";
        }
    } else {
        qWarning() << "Failed to download Facebook avatar:" << reply->errorString();
    }

    reply->deleteLater();
}

void LinkedAccountsTab::onFacebookPagesFetched(const QList<FacebookPageData>& pages) {
    if (isFacebookDebugLoggingEnabled(linkedAccountsLog())) {
        qCDebug(linkedAccountsLog) << "Facebook pages fetched" << pages.size();
    }
    m_facebookPages = pages;
    m_facebookPagesComboBox->clear();
    for (const auto& page : pages) {
        m_facebookPagesComboBox->addItem(page.name, page.id);
    }
}

void LinkedAccountsTab::updateTheme()
{
    Theme t = ThemeManager::instance()->currentTheme();
    const QColor defaultText = palette().color(QPalette::Text);
    const QColor textColor = t.textColor.isValid() ? t.textColor : (t.text.isValid() ? t.text : defaultText);
    QColor mutedText = textColor;
    mutedText.setAlphaF(0.75);
    const QColor accent = t.primary.isValid() ? t.primary : (t.accent.isValid() ? t.accent : textColor);

    for (QLabel *heading : m_headingLabels) {
        heading->setStyleSheet(QStringLiteral("color: %1; font-weight: 600;").arg(textColor.name()));
    }

    for (QLabel *caption : m_captionLabels) {
        caption->setStyleSheet(QStringLiteral("color: %1;").arg(mutedText.name(QColor::HexArgb)));
    }

    if (m_googleAccountStatusLabel) {
        m_googleAccountStatusLabel->setStyleSheet(QStringLiteral("color: %1; font-weight: 500;").arg(mutedText.darker(115).name(QColor::HexArgb)));
    }

    if (QLabel *pageLabel = m_facebookProfileCard->findChild<QLabel*>("facebookPageLabel")) {
        pageLabel->setStyleSheet(QStringLiteral("color: %1; font-weight: 500;").arg(textColor.name()));
    }

    styleButton(m_linkGoogleAccountButton);
    styleButton(m_linkFacebookAccountButton);

    if (m_googleCard) {
        m_googleCard->updateTheme();
    }
    if (m_facebookCard) {
        m_facebookCard->updateTheme();
    }
}

QWidget *LinkedAccountsTab::createSectionHeader(const QString &iconPath, const QString &title)
{
    QWidget *header = new QWidget(this);
    auto *layout = new QHBoxLayout(header);
    layout->setContentsMargins(0, 0, 0, 0);
    layout->setSpacing(8);

    auto *iconLabel = new QLabel(header);
    iconLabel->setPixmap(QPixmap(iconPath).scaled(24, 24, Qt::KeepAspectRatio, Qt::SmoothTransformation));

    auto *titleLabel = new QLabel(title, header);
    QFont font = titleLabel->font();
    font.setPointSize(font.pointSize() + 2);
    font.setBold(true);
    titleLabel->setFont(font);
    titleLabel->setSizePolicy(QSizePolicy::Preferred, QSizePolicy::Preferred);

    layout->addWidget(iconLabel);
    layout->addWidget(titleLabel);
    layout->addStretch();

    m_headingLabels.append(titleLabel);
    return header;
}

QLabel *LinkedAccountsTab::createCaptionLabel(const QString &text)
{
    auto *caption = new QLabel(text, this);
    caption->setWordWrap(true);
    QFont font = caption->font();
    const int pointSize = font.pointSize();
    if (pointSize > 0) {
        font.setPointSize(std::max(9, pointSize - 1));
    }
    caption->setFont(font);
    m_captionLabels.append(caption);
    return caption;
}
