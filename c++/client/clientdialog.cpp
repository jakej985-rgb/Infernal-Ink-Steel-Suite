#include "clientdialog.h"
#include <QFormLayout>
#include <QVBoxLayout>
#include <QEasingCurve>

ClientDialog::ClientDialog(QWidget *parent)
    : ThemeableDialog(parent),
    mousePressed(false),
    themeManager(ThemeManager::instance())
{
    setWindowTitle("Add New Client");
    setFixedSize(450, 300);
    setWindowFlags(Qt::FramelessWindowHint | Qt::Dialog);
    setWindowOpacity(0.0);

    firstNameEdit = new QLineEdit(this);
    middleNameEdit = new QLineEdit(this);
    lastNameEdit = new QLineEdit(this);
    emailEdit = new QLineEdit(this);
    phoneEdit = new QLineEdit(this);

    okButton = new QPushButton("Add Client", this);
    cancelButton = new QPushButton("Cancel", this);

    QFormLayout *formLayout = new QFormLayout;
    formLayout->addRow("First Name:", firstNameEdit);
    formLayout->addRow("Middle Name:", middleNameEdit);
    formLayout->addRow("Last Name:", lastNameEdit);
    formLayout->addRow("Email:", emailEdit);
    formLayout->addRow("Phone:", phoneEdit);

    QHBoxLayout *buttonLayout = new QHBoxLayout;
    buttonLayout->addStretch();
    buttonLayout->addWidget(okButton);
    buttonLayout->addWidget(cancelButton);

    QVBoxLayout *mainLayout = new QVBoxLayout(this);
    mainLayout->setContentsMargins(20, 20, 20, 20);
    mainLayout->setSpacing(15);
    mainLayout->addLayout(formLayout);
    mainLayout->addLayout(buttonLayout);
    setLayout(mainLayout);

    dialogGlowEffect = new QGraphicsDropShadowEffect(this);
    dialogGlowEffect->setBlurRadius(30);
    dialogGlowEffect->setOffset(0, 0);
    setGraphicsEffect(dialogGlowEffect);

    fadeIn();

    buttonGlowOk = new GlowEffect(okButton, true, this);
    buttonGlowCancel = new GlowEffect(cancelButton, true, this);

    connect(okButton, &QPushButton::clicked, this, &ClientDialog::accept);
    connect(cancelButton, &QPushButton::clicked, this, &ClientDialog::reject);
}

// ---------------- THEME ----------------
void ClientDialog::updateTheme()
{
    ThemeableDialog::updateTheme(); // Call base class implementation
    Theme t = themeManager->currentTheme();
    dialogGlowEffect->setColor(t.accent);
}

// ---------------- FADE & EVENTS ----------------
void ClientDialog::fadeIn()
{
    fadeAnimation = new QPropertyAnimation(this, "windowOpacity", this);
    fadeAnimation->setDuration(250);
    fadeAnimation->setStartValue(0.0);
    fadeAnimation->setEndValue(1.0);
    fadeAnimation->start();
}

void ClientDialog::fadeOut()
{
    fadeAnimation = new QPropertyAnimation(this, "windowOpacity", this);
    fadeAnimation->setDuration(200);
    fadeAnimation->setStartValue(1.0);
    fadeAnimation->setEndValue(0.0);
    connect(fadeAnimation, &QPropertyAnimation::finished, this, &QDialog::close);
    fadeAnimation->start();
}

void ClientDialog::mousePressEvent(QMouseEvent *event)
{
    if (event->button() == Qt::LeftButton) {
        mousePressed = true;
        mousePressPos = event->globalPosition().toPoint() - frameGeometry().topLeft();
    }
}

void ClientDialog::mouseMoveEvent(QMouseEvent *event)
{
    if (mousePressed)
        move(event->globalPosition().toPoint() - mousePressPos);
}

void ClientDialog::mouseReleaseEvent(QMouseEvent *event)
{
    Q_UNUSED(event)
    mousePressed = false;
}

void ClientDialog::accept()
{
    fadeOut();
    QDialog::accept();
}

void ClientDialog::reject()
{
    fadeOut();
    QDialog::reject();
}

// ---------------- CLIENT DATA ----------------
QString ClientDialog::getFirstName() const { return firstNameEdit->text(); }
QString ClientDialog::getMiddleName() const { return middleNameEdit->text(); }
QString ClientDialog::getLastName() const { return lastNameEdit->text(); }
QString ClientDialog::getEmail() const { return emailEdit->text(); }
QString ClientDialog::getPhone() const { return phoneEdit->text(); }

Client ClientDialog::getClient() const {
    Client c;
    c.firstName = getFirstName();
    c.middleName = getMiddleName();
    c.lastName = getLastName();
    c.email = getEmail();
    c.phone = getPhone();
    return c;
}
