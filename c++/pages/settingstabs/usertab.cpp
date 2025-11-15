#include "usertab.h"
#include "helper/thememanager.h"
#include "themes/theme.h"
#include "widgets/settingscard.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QLabel>
#include <QLineEdit>
#include <QPushButton>
#include <QMessageBox>
#include <QPixmap>
#include <QPalette>
#include <QSizePolicy>
#include <algorithm>

UserTab::UserTab(UserDB &udb, const QString &uname, QWidget *parent)
    : ThemeableWidget(parent), userDB(udb), username(uname)
{
    auto *mainLayout = new QVBoxLayout(this);
    mainLayout->setContentsMargins(24, 24, 24, 24);
    mainLayout->setSpacing(24);

    userCard = new SettingsCard(this);
    auto *cardLayout = userCard->contentLayout();

    cardLayout->addWidget(createSectionHeader(":/icons/login.png", tr("Password Security")));
    cardLayout->addWidget(createCaptionLabel(tr("Update your password to keep your account secure.")));

    newPass = new QLineEdit(userCard);
    newPass->setEchoMode(QLineEdit::Password);
    changeBtn = new QPushButton(tr("Change Password"), userCard);

    auto *fieldLayout = new QVBoxLayout;
    fieldLayout->setContentsMargins(0, 0, 0, 0);
    fieldLayout->setSpacing(8);
    auto *fieldLabel = new QLabel(tr("New Password"), userCard);
    fieldLabel->setObjectName("fieldLabel");
    cardLayout->addLayout(fieldLayout);
    fieldLayout->addWidget(fieldLabel);
    fieldLayout->addWidget(newPass);

    auto *buttonLayout = new QHBoxLayout;
    buttonLayout->setContentsMargins(0, 0, 0, 0);
    buttonLayout->setSpacing(12);
    buttonLayout->addStretch();
    buttonLayout->addWidget(changeBtn);
    cardLayout->addLayout(buttonLayout);

    connect(changeBtn, &QPushButton::clicked, this, [=]() {
        QString pwd = newPass->text();
        if (pwd.isEmpty()) return;
        userDB.updatePassword(username, pwd);
        QMessageBox::information(this, tr("Updated"), tr("Password changed successfully."));
        newPass->clear();
    });

    mainLayout->addWidget(userCard);
    mainLayout->addStretch();
    updateTheme();
}

void UserTab::updateTheme()
{
    Theme t = ThemeManager::instance()->currentTheme();
    const QColor defaultText = palette().color(QPalette::Text);
    const QColor textColor = t.textColor.isValid() ? t.textColor : (t.text.isValid() ? t.text : defaultText);
    QColor mutedText = textColor;
    mutedText.setAlphaF(0.75);
    const QColor accent = t.primary.isValid() ? t.primary : (t.accent.isValid() ? t.accent : textColor);
    const QColor fieldBg = t.background.isValid() ? t.background.lighter(110) : palette().color(QPalette::Base);
    const QColor border = t.borderColor.isValid() ? t.borderColor : (t.border.isValid() ? t.border : accent.darker(115));

    for (QLabel *heading : headingLabels) {
        heading->setStyleSheet(QStringLiteral("color: %1; font-weight: 600;").arg(textColor.name()));
    }

    for (QLabel *caption : captionLabels) {
        caption->setStyleSheet(QStringLiteral("color: %1;").arg(mutedText.name(QColor::HexArgb)));
    }

    const QString fieldStyle = QStringLiteral(
        "QLineEdit {"
        "    background-color: %1;"
        "    border: 1px solid %2;"
        "    border-radius: 10px;"
        "    padding: 6px 10px;"
        "    color: %3;"
        "}"
        "QLineEdit:focus {"
        "    border-color: %4;"
        "}")
        .arg(fieldBg.name(QColor::HexArgb))
        .arg(border.name())
        .arg(textColor.name())
        .arg(accent.name());

    newPass->setStyleSheet(fieldStyle);
    if (QLabel *fieldLabel = userCard->findChild<QLabel*>("fieldLabel")) {
        fieldLabel->setStyleSheet(QStringLiteral("color: %1; font-weight: 500;").arg(textColor.name()));
    }

    styleButton(changeBtn);
    if (userCard) {
        userCard->updateTheme();
    }
}

QWidget *UserTab::createSectionHeader(const QString &iconPath, const QString &title)
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

    headingLabels.append(titleLabel);
    return header;
}

QLabel *UserTab::createCaptionLabel(const QString &text)
{
    auto *caption = new QLabel(text, this);
    caption->setWordWrap(true);
    QFont font = caption->font();
    const int pointSize = font.pointSize();
    if (pointSize > 0) {
        font.setPointSize(std::max(9, pointSize - 1));
    }
    caption->setFont(font);
    captionLabels.append(caption);
    return caption;
}
