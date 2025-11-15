#include "login.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QMessageBox>
#include <QPainter>
#include <QFrame>
#include <QGraphicsDropShadowEffect>
#include <QFileInfo>
#include <QSizePolicy>
#include "dashboard.h"
#include "helper/thememanager.h"

// --- Neon glow effect helper ---
static void applyNeonGlow(QWidget *widget, const QColor &color, int blurRadius = 20, int offset = 0) {
    auto *effect = new QGraphicsDropShadowEffect(widget);
    effect->setBlurRadius(blurRadius);
    effect->setColor(color);
    effect->setOffset(offset);
    widget->setGraphicsEffect(effect);
}

// --- Avatar helper ---
QPixmap Login::makeLetterPixmap(const QString &initial, int size, const QColor &base) const {
    QPixmap pixmap(size, size);
    pixmap.fill(Qt::transparent);
    QPainter p(&pixmap);
    p.setRenderHint(QPainter::Antialiasing);
    p.setBrush(base);
    p.setPen(Qt::NoPen);
    p.drawEllipse(0, 0, size, size);

    p.setPen(Qt::white);
    QFont f("Segoe UI", size / 2, QFont::Bold);
    p.setFont(f);
    p.drawText(pixmap.rect(), Qt::AlignCenter, initial);
    return pixmap;
}

// --- Constructor ---
Login::Login(UserDB *udb, ShopSettingDB *sdb, QWidget *parent)
    : ThemeableWidget(parent)
    , userDB(udb)
    , settingsDb(sdb)
{
    setObjectName("loginView");
    setWindowTitle("Login");
    m_brandAccent = QColor();
    m_backgroundPath.clear();
    resize(900, 620);

    auto *root = new QVBoxLayout(this);
    root->setContentsMargins(24,24,24,24);
    root->setSpacing(16);

    m_brandingPanel = new QWidget(this);
    m_brandingPanel->setAttribute(Qt::WA_StyledBackground, false);
    m_brandingPanel->setStyleSheet("background-color: transparent;");
    auto *brandingLayout = new QVBoxLayout(m_brandingPanel);
    brandingLayout->setContentsMargins(8, 0, 8, 12);
    brandingLayout->setSpacing(4);
    m_headlineLabel = new QLabel(m_brandingPanel);
    m_headlineLabel->setObjectName("loginHeadlineLabel");
    m_headlineLabel->setWordWrap(true);
    QFont headlineFont = m_headlineLabel->font();
    headlineFont.setPointSize(headlineFont.pointSize() + 6);
    headlineFont.setBold(true);
    m_headlineLabel->setFont(headlineFont);
    m_headlineLabel->setText(tr("Welcome Back"));
    m_taglineLabel = new QLabel(m_brandingPanel);
    m_taglineLabel->setObjectName("loginTaglineLabel");
    m_taglineLabel->setWordWrap(true);
    m_taglineLabel->setText(tr("Sign in to manage your day."));
    brandingLayout->addWidget(m_headlineLabel);
    brandingLayout->addWidget(m_taglineLabel);
    root->addWidget(m_brandingPanel);

    // --- User selection grid ---
    m_gridContainer = new QWidget(this);
    m_gridContainer->setAttribute(Qt::WA_StyledBackground, false);
    m_gridContainer->setStyleSheet("background-color: transparent;");
    m_grid = new QGridLayout(m_gridContainer);
    m_grid->setContentsMargins(8,8,8,8);
    m_grid->setHorizontalSpacing(18);
    m_grid->setVerticalSpacing(22);

    m_userScrollArea = new QScrollArea(this);
    m_userScrollArea->setWidgetResizable(true);
    m_userScrollArea->setFrameShape(QFrame::NoFrame);
    m_userScrollArea->setHorizontalScrollBarPolicy(Qt::ScrollBarAlwaysOff);
    m_userScrollArea->setWidget(m_gridContainer);
    m_userScrollArea->setStyleSheet("background-color: transparent; border: none;");
    if (auto *viewport = m_userScrollArea->viewport()) {
        viewport->setStyleSheet("background-color: transparent; border: none;");
    }
    root->addWidget(m_userScrollArea, 1);
    buildUserGrid();

    // --- Password + login view ---
    m_userLoginView = new QWidget(this);
    m_userLoginView->setAttribute(Qt::WA_StyledBackground, false);
    m_userLoginView->setStyleSheet("background-color: transparent;");
    m_userLoginView->setSizePolicy(QSizePolicy::Preferred, QSizePolicy::Expanding);
    auto *centerLayout = new QVBoxLayout(m_userLoginView);
    centerLayout->setAlignment(Qt::AlignHCenter);
    centerLayout->setSpacing(18);
    centerLayout->setContentsMargins(0, 0, 0, 0);

    int avatarSize = 140;
    m_avatarLabel = new GlowAvatar(m_userLoginView);
    m_avatarLabel->setSize(avatarSize);
    m_avatarLabel->setPixmap(makeLetterPixmap("U", avatarSize));
    m_userHeaderRow = new QWidget(m_userLoginView);
    m_userHeaderRow->setAttribute(Qt::WA_StyledBackground, false);
    auto *headerLayout = new QVBoxLayout(m_userHeaderRow);
    headerLayout->setContentsMargins(0, 0, 0, 0);
    headerLayout->setSpacing(12);
    headerLayout->setAlignment(Qt::AlignHCenter);

    headerLayout->addWidget(m_avatarLabel, 0, Qt::AlignHCenter);

    m_selectedUserLabel = new QLabel("Select User", m_userHeaderRow);
    m_selectedUserLabel->setAlignment(Qt::AlignHCenter | Qt::AlignVCenter);
    headerLayout->addWidget(m_selectedUserLabel, 0, Qt::AlignHCenter);

    centerLayout->addStretch(1);
    centerLayout->addWidget(m_userHeaderRow, 0, Qt::AlignHCenter);
    centerLayout->addSpacing(12);

    m_passwordEdit = new QLineEdit(m_userLoginView);
    m_passwordEdit->setEchoMode(QLineEdit::Password);
    m_passwordEdit->setFixedWidth(300);
    m_passwordEdit->setAlignment(Qt::AlignCenter);
    centerLayout->addWidget(m_passwordEdit, 0, Qt::AlignHCenter);

    m_showPasswordCheck = new QCheckBox("Show Password", m_userLoginView);
    connect(m_showPasswordCheck, &QCheckBox::toggled, this, &Login::toggleShowPassword);
    centerLayout->addWidget(m_showPasswordCheck, 0, Qt::AlignHCenter);

    m_signInBtn = new NeonButton("Sign In", QIcon(":/icons/login.png"), m_userLoginView);
    m_backBtn   = new NeonButton("Back", QIcon(":/icons/back.png"), m_userLoginView);

    m_signInBtn->setFixedHeight(44);
    m_backBtn->setFixedHeight(44);

    auto *hBtn = new QHBoxLayout;
    hBtn->addWidget(m_backBtn);
    hBtn->addWidget(m_signInBtn);
    centerLayout->addLayout(hBtn);
    centerLayout->setAlignment(hBtn, Qt::AlignHCenter);

    centerLayout->addStretch(1);

    root->addWidget(m_userLoginView);
    m_userLoginView->hide();

    // --- Connections ---
    connect(m_backBtn, &NeonButton::clicked, this, &Login::onBackClicked);
    connect(m_signInBtn, &NeonButton::clicked, this, &Login::onSignInClicked);
    connect(m_passwordEdit, &QLineEdit::returnPressed, this, &Login::onSignInClicked);

    applyBranding();
    updateTheme();
}

// --- Build user grid ---
void Login::buildUserGrid() {
    while (QLayoutItem *item = m_grid->takeAt(0)) {
        if (QWidget *w = item->widget())
            w->deleteLater();
        delete item;
    }

    int row = 0, col = 0;
    const int maxCols = 3;
    const int avatarSize = 100;

    const QList<User> users = userDB->getAllUsers();
    if (users.isEmpty()) {
        QLabel *emptyLabel = new QLabel("No users available.", m_gridContainer);
        emptyLabel->setAlignment(Qt::AlignCenter);
        m_grid->addWidget(emptyLabel, 0, 0, 1, maxCols);
        return;
    }

    for (const auto &u : users) {
        QWidget *userWidget = new QWidget(m_gridContainer);
        userWidget->setAttribute(Qt::WA_StyledBackground, false);
        userWidget->setStyleSheet("background-color: transparent;");
        QVBoxLayout *vLayout = new QVBoxLayout(userWidget);
        vLayout->setContentsMargins(0,0,0,0);
        vLayout->setSpacing(6);
        vLayout->setAlignment(Qt::AlignCenter);

        QPixmap pm = makeLetterPixmap(u.username.left(1).toUpper(), avatarSize);
        GlowAvatar *avatar = new GlowAvatar(userWidget);
        avatar->setSize(avatarSize);
        avatar->setPixmap(pm);
        avatar->setRole(u.role);
        vLayout->addWidget(avatar, 0, Qt::AlignHCenter);

        QLabel *nameLabel = new QLabel(u.username, userWidget);
        nameLabel->setAlignment(Qt::AlignCenter);
        vLayout->addWidget(nameLabel);

        connect(avatar, &GlowAvatar::clicked, this, [this, u]() { showUserSelected(u); });

        m_grid->addWidget(userWidget, row, col, Qt::AlignCenter);
        if (++col >= maxCols) { col = 0; ++row; }
    }
}

void Login::applyBranding()
{
    if (!settingsDb) {
        return;
    }

    const ShopSettings settings = settingsDb->loadSettings();

    const QString shopName = settings.shopName.trimmed();
    setWindowTitle(shopName.isEmpty() ? tr("Login") : tr("%1 Login").arg(shopName));

    if (m_headlineLabel) {
        const QString headline = settings.loginHeadline.trimmed();
        m_headlineLabel->setText(headline.isEmpty() ? tr("Welcome Back") : headline);
    }
    if (m_taglineLabel) {
        const QString tagline = settings.loginTagline.trimmed();
        m_taglineLabel->setText(tagline.isEmpty() ? tr("Sign in to manage your day.") : tagline);
    }

    m_brandAccent = settings.accentColor.isEmpty() ? QColor() : QColor(settings.accentColor);
    if (!m_brandAccent.isValid()) {
        m_brandAccent = QColor();
    }

    m_backgroundPath = settings.loginBackgroundPath;

    if (m_brandAccent.isValid()) {
        if (m_signInBtn) {
            m_signInBtn->setGlowColor(m_brandAccent);
        }
        if (m_backBtn) {
            m_backBtn->setGlowColor(m_brandAccent.darker(115));
        }
    }
}

// --- Show selected user ---
void Login::showUserSelected(const User &user) {
    m_currentUser = user;
    int avatarSize = m_avatarLabel->width();
    if (avatarSize <= 0) {
        avatarSize = m_avatarLabel->height();
    }
    if (avatarSize <= 0) {
        avatarSize = 140;
    }
    QPixmap pm = makeLetterPixmap(user.username.left(1).toUpper(), avatarSize);
    m_avatarLabel->setPixmap(pm);
    m_avatarLabel->setRole(user.role);
    m_selectedUserLabel->setText(user.username);
    m_passwordEdit->clear();

    if (m_userScrollArea)
        m_userScrollArea->hide();
    m_userLoginView->show();
    m_passwordEdit->setFocus();
}

// --- Sign in ---
void Login::onSignInClicked()
{
    QString password = m_passwordEdit->text().trimmed();
    if (password.isEmpty()) {
        QMessageBox::warning(this, "Error", "Please enter your password.");
        return;
    }

    // ✅ Fetch user from DB
    User user = userDB->getUserByUsername(m_currentUser.username);

    if (user.username.isEmpty()) {
        QMessageBox::warning(this, "Error", "User not found!");
        return;
    }

    // ✅ Check password (plain-text or hash depending on your DB)
    // If your UserDB stores hashed passwords, compare hashed versions:
    QString hashedInput = QString(QCryptographicHash::hash(password.toUtf8(), QCryptographicHash::Sha256).toHex());
    if (user.passwordHash != hashedInput && user.passwordHash != password) {
        QMessageBox::warning(this, "Error", "Incorrect password.");
        return;
    }

    // ✅ Open Dashboard using DatabaseManager singleton
    DatabaseManager *dbMgr = DatabaseManager::instance();
    if (!dbMgr) {
        QMessageBox::critical(this, "Error", "Database not initialized.");
        return;
    }

    // ✅ Create per-user DB handles
    m_clientDb.reset(new ClientDB(&dbMgr->getDatabase()));
    m_appointmentDb.reset(new AppointmentDB(&dbMgr->getDatabase()));
    m_documentsDb.reset(new DocumentStorageDB(&dbMgr->getDatabase(), "documents"));

    // ✅ Create and show dashboard
    Dashboard *dash = new Dashboard(
        user.username,
        user.role,
        &dbMgr->settings(),
        &dbMgr->users(),
        m_clientDb.data(),
        m_appointmentDb.data(),
        m_documentsDb.data(),
        nullptr
        );

    connect(dash, &Dashboard::logoutRequested, this, &Login::showAfterLogout);

    dash->setAttribute(Qt::WA_DeleteOnClose);
    dash->show();
    this->hide();
}

// --- After logout ---
void Login::showAfterLogout() {
    m_clientDb.reset();
    m_appointmentDb.reset();
    m_documentsDb.reset();
    buildUserGrid();
    if (m_userLoginView) {
        m_userLoginView->hide();
    }
    if (m_userScrollArea) {
        m_userScrollArea->show();
    }
    applyBranding();
    updateTheme();
    this->show();
}

// --- Toggle password visibility ---
void Login::toggleShowPassword() {
    m_passwordEdit->setEchoMode(m_showPasswordCheck->isChecked()
                                ? QLineEdit::Normal
                                : QLineEdit::Password);
}

// --- Back button ---
void Login::onBackClicked() {
    m_userLoginView->hide();
    if (m_userScrollArea)
        m_userScrollArea->show();
}

void Login::updateTheme() {
    Theme t = ThemeManager::instance()->currentTheme();

    QColor baseBackground = t.background.isValid() ? t.background : palette().color(QPalette::Window);
    QColor textColor = t.text.isValid() ? t.text : palette().color(QPalette::WindowText);
    QColor accent = m_brandAccent.isValid() ? m_brandAccent : (t.accent.isValid() ? t.accent : t.primary);
    if (!accent.isValid()) {
        accent = textColor;
    }

    QString baseStyle = QStringLiteral("#loginView { background-color:%1; color:%2;")
        .arg(baseBackground.name(QColor::HexArgb), textColor.name());
    if (!m_backgroundPath.isEmpty() && QFileInfo::exists(m_backgroundPath)) {
        baseStyle += QStringLiteral(
            " background-image: url(%1);"
            " background-position: top right;"
            " background-repeat: no-repeat;"
            " background-size: 420px auto;"
        ).arg(m_backgroundPath);
    }
    baseStyle += "}";
    setStyleSheet(baseStyle);

    if (m_headlineLabel) {
        m_headlineLabel->setStyleSheet(QStringLiteral("color:%1; background-color: transparent;").arg(textColor.name()));
    }
    if (m_taglineLabel) {
        QColor taglineColor = textColor;
        taglineColor.setAlpha(204); // 0.8 * 255 to avoid float truncation warnings on MSVC builds
        m_taglineLabel->setStyleSheet(QStringLiteral("color:%1; background-color: transparent;")
                                      .arg(taglineColor.name(QColor::HexArgb)));
    }

    if (m_selectedUserLabel) {
        QColor badgeBg = accent.isValid() ? accent : textColor;
        if (!badgeBg.isValid()) {
            badgeBg = textColor;
        }
        if (badgeBg.alpha() == 0) {
            badgeBg.setAlpha(255);
        }
        badgeBg.setAlpha(230);
        QColor badgeBorder = badgeBg.darker(140);
        QColor badgeText = baseBackground.isValid() ? baseBackground : QColor(Qt::white);
        const qreal luminance = 0.299 * badgeBg.redF() + 0.587 * badgeBg.greenF() + 0.114 * badgeBg.blueF();
        if (luminance < 0.55) {
            badgeText = QColor(Qt::white);
        } else {
            badgeText = QColor(Qt::black);
        }
        m_selectedUserLabel->setStyleSheet(QStringLiteral(
            "color:%1; font-size:20px; font-weight:600; background-color:%2;"
            " padding:6px 14px; border-radius:14px; border:1px solid %3;"
        ).arg(badgeText.name(QColor::HexArgb),
              badgeBg.name(QColor::HexArgb),
              badgeBorder.name(QColor::HexArgb)));
    }

    QColor fieldBackground = baseBackground.lighter(115);
    QString passwordStyle = QStringLiteral(
        "QLineEdit { border:2px solid %1; border-radius:12px; padding:8px;"
        "background-color:%2; color:%3; font-size:16px; }"
        "QLineEdit:focus { border:2px solid %4; }")
        .arg(accent.name())
        .arg(fieldBackground.name(QColor::HexArgb))
        .arg(textColor.name())
        .arg(accent.lighter(120).name());
    if (m_passwordEdit) {
        m_passwordEdit->setStyleSheet(passwordStyle);
        applyNeonGlow(m_passwordEdit, accent, 25, 0);
    }

    if (m_showPasswordCheck) {
        m_showPasswordCheck->setStyleSheet(QStringLiteral("color:%1;").arg(textColor.name()));
    }

    QString btnStyle = QStringLiteral(
        "NeonButton { border-radius:10px; background-color:qlineargradient(x1:0,y1:0,x2:0,y2:1, stop:0 %1, stop:1 %2);"
        " color:%3; font-size:15px; padding:6px; }"
        "NeonButton:hover { background-color:qlineargradient(x1:0,y1:0,x2:0,y2:1, stop:0 %4, stop:1 %5); }"
        "NeonButton:pressed { background-color:%6; }")
        .arg(accent.lighter(130).name())
        .arg(accent.name())
        .arg(textColor.name())
        .arg(accent.lighter(160).name())
        .arg(accent.lighter(130).name())
        .arg(accent.darker(130).name());
    if (m_signInBtn) {
        m_signInBtn->setStyleSheet(btnStyle);
        m_signInBtn->setGlowColor(accent);
    }
    if (m_backBtn) {
        m_backBtn->setStyleSheet(btnStyle);
        m_backBtn->setGlowColor(accent);
    }

    if (m_gridContainer) {
        const QList<QLabel*> labels = m_gridContainer->findChildren<QLabel *>();
        for (QLabel *lbl : labels) {
            lbl->setStyleSheet(QStringLiteral("color:%1; font-size:14px; background-color: transparent;")
                               .arg(textColor.name()));
        }
    }
}

// --- Resize handler ---
void Login::resizeEvent(QResizeEvent *event) {
    QWidget::resizeEvent(event);
}
