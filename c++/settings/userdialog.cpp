#include "userdialog.h"
#include "avatardialog.h"
#include "db/userdb.h"
#include "helper/thememanager.h"
#include "themes/themeablewidget.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QLabel>
#include <QLineEdit>
#include <QComboBox>
#include <QPushButton>
#include <QGraphicsDropShadowEffect>
#include <QPropertyAnimation>
#include <QEasingCurve>
#include <QMessageBox>
#include <QPixmap>
#include <QMouseEvent>
#include <QCryptographicHash>

UserDialog::UserDialog(QSqlDatabase* db, bool isAdmin, QWidget* parent)
    : ThemeableDialog(parent),
    m_db(db),
    m_isAdmin(isAdmin)
{
    setWindowFlags(Qt::FramelessWindowHint | Qt::Dialog);
    setWindowOpacity(0.0);
    setFixedSize(420, 360);
    setWindowTitle("User Editor");

    setupUi();
    updateTheme();
    fadeIn();
}

void UserDialog::setupUi()
{
    auto *mainLayout = new QVBoxLayout(this);
    mainLayout->setContentsMargins(20, 20, 20, 20);
    mainLayout->setSpacing(15);

    auto *titleLabel = new QLabel("Add / Edit User", this);
    titleLabel->setAlignment(Qt::AlignCenter);
    titleLabel->setObjectName("ud_title");
    mainLayout->addWidget(titleLabel);

    usernameEdit = new QLineEdit(this);
    usernameEdit->setPlaceholderText("Username");

    passwordEdit = new QLineEdit(this);
    passwordEdit->setPlaceholderText("Password");
    passwordEdit->setEchoMode(QLineEdit::Password);

    roleComboBox = new QComboBox(this);
    roleComboBox->addItems({"Artist", "Piercer", "Reception", "Admin"});
    roleComboBox->setEnabled(m_isAdmin);

    avatarPreview = new QLabel(this);
    avatarPreview->setFixedSize(80, 80);
    avatarPreview->setAlignment(Qt::AlignCenter);
    avatarPreview->setPixmap(QPixmap(":/avatars/a1.png").scaled(72, 72, Qt::KeepAspectRatio, Qt::SmoothTransformation));

    avatarButton = new QPushButton("Choose Avatar", this);
    connect(avatarButton, &QPushButton::clicked, this, &UserDialog::openAvatarDialog);

    QHBoxLayout *avatarLayout = new QHBoxLayout();
    avatarLayout->addWidget(avatarPreview);
    avatarLayout->addWidget(avatarButton);
    mainLayout->addLayout(avatarLayout);

    mainLayout->addWidget(usernameEdit);
    mainLayout->addWidget(passwordEdit);
    mainLayout->addWidget(roleComboBox);

    saveButton = new QPushButton("Save", this);
    cancelButton = new QPushButton("Cancel", this);

    QHBoxLayout *buttonLayout = new QHBoxLayout();
    buttonLayout->addStretch();
    buttonLayout->addWidget(cancelButton);
    buttonLayout->addWidget(saveButton);
    mainLayout->addLayout(buttonLayout);

    connect(saveButton, &QPushButton::clicked, this, &UserDialog::saveUser);
    connect(cancelButton, &QPushButton::clicked, this, &UserDialog::reject);

    // Glow effects
    glowSave = new GlowEffect(saveButton, true, this);
    glowCancel = new GlowEffect(cancelButton, true, this);

    // Dialog glow
    dialogGlowEffect = new QGraphicsDropShadowEffect(this);
    dialogGlowEffect->setBlurRadius(30);
    dialogGlowEffect->setOffset(0, 0);
    dialogGlowEffect->setColor(QColor(0, 170, 255, 80));
    setGraphicsEffect(dialogGlowEffect);
}

void UserDialog::updateTheme()
{
    Theme t = ThemeManager::instance()->currentTheme();

    setStyleSheet(QString("QDialog { background-color:%1; border-radius:15px; }").arg(t.background.name()));

    QString editStyle = QString(
                            "QLineEdit, QComboBox { background-color:%1; border:2px solid %2; border-radius:8px; padding:6px; color:%3; }"
                            "QLineEdit:focus, QComboBox:focus { border:2px solid %4; }")
                            .arg(t.background.darker(130).name(),
                                 t.primary.name(),
                                 t.text.name(),
                                 t.accent.name());

    usernameEdit->setStyleSheet(editStyle);
    passwordEdit->setStyleSheet(editStyle);
    roleComboBox->setStyleSheet(editStyle);

    QString btnStyle = QString(
                           "QPushButton { background-color:%1; color:%2; border-radius:8px; padding:6px 10px; font-weight:bold; }"
                           "QPushButton:hover { background-color:%3; border:2px solid %4; }")
                           .arg(t.primary.name(),
                                t.text.name(),
                                t.primary.darker(120).name(),
                                t.accent.name());

    saveButton->setStyleSheet(btnStyle);
    cancelButton->setStyleSheet(btnStyle);

    if (auto title = findChild<QLabel*>("ud_title"))
        title->setStyleSheet(QString("color:%1; font-weight:bold; font-size:18px;").arg(t.text.name()));
}

void UserDialog::setUser(const User &user)
{
    m_user = user;
    usernameEdit->setText(user.username);
    roleComboBox->setCurrentText(user.role);
    // We don't store avatar in User struct → no avatarPath handling
}

void UserDialog::saveUser()
{
    QString username = usernameEdit->text().trimmed();
    QString password = passwordEdit->text();
    QString role = roleComboBox->currentText();

    if (username.isEmpty() || password.isEmpty()) {
        QMessageBox::warning(this, "Missing Data", "Please fill in all required fields.");
        return;
    }

    QString passwordHash = QString::fromUtf8(QCryptographicHash::hash(password.toUtf8(), QCryptographicHash::Sha256).toHex());

    UserDB userDb(m_db);
    m_user.username = username;
    m_user.passwordHash = passwordHash;
    m_user.role = role;

    if (m_user.id == -1) {
        // Your addUser likely takes three arguments
        if (!userDb.addUser(username, passwordHash, role)) {
            QMessageBox::critical(this, "Error", "Failed to add user.");
            return;
        }
    } else {
        userDb.updateUser(m_user);
    }

    emit userSaved(m_user);
    accept();
}

void UserDialog::openAvatarDialog()
{
    AvatarDialog dlg(this);
    if (dlg.exec() == QDialog::Accepted) {
        m_avatarPath = dlg.selectedPath();
        avatarPreview->setPixmap(QPixmap(m_avatarPath).scaled(72, 72, Qt::KeepAspectRatio, Qt::SmoothTransformation));
    }
}

void UserDialog::fadeIn()
{
    fadeAnimation = new QPropertyAnimation(this, "windowOpacity", this);
    fadeAnimation->setDuration(250);
    fadeAnimation->setStartValue(0.0);
    fadeAnimation->setEndValue(1.0);
    fadeAnimation->start(QAbstractAnimation::DeleteWhenStopped);
}

void UserDialog::fadeOut()
{
    fadeAnimation = new QPropertyAnimation(this, "windowOpacity", this);
    fadeAnimation->setDuration(200);
    fadeAnimation->setStartValue(1.0);
    fadeAnimation->setEndValue(0.0);
    connect(fadeAnimation, &QPropertyAnimation::finished, this, &QDialog::close);
    fadeAnimation->start(QAbstractAnimation::DeleteWhenStopped);
}

void UserDialog::mousePressEvent(QMouseEvent *event)
{
    if (event->button() == Qt::LeftButton) {
        mousePressed = true;
        mousePressPos = event->globalPosition().toPoint() - frameGeometry().topLeft();
    }
}

void UserDialog::mouseMoveEvent(QMouseEvent *event)
{
    if (mousePressed)
        move(event->globalPosition().toPoint() - mousePressPos);
}

void UserDialog::mouseReleaseEvent(QMouseEvent * /*event*/)
{
    mousePressed = false;
}

void UserDialog::accept()
{
    fadeOut();
    QDialog::accept();
}

void UserDialog::reject()
{
    fadeOut();
    QDialog::reject();
}
