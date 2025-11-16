#include "duplicateclientdialog.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QLabel>
#include <QLineEdit>
#include <QPushButton>
#include <QGraphicsDropShadowEffect>
#include <QPropertyAnimation>
#include <QMouseEvent>
#include <QFormLayout>
#include "helper/gloweffect.h"
#include "helper/thememanager.h"

DuplicateClientDialog::DuplicateClientDialog(const QString &existingFirstName,
                                             const QString &existingMiddleName,
                                             const QString &existingLastName,
                                             const QString &existingPhone,
                                             const QString &existingEmail,
                                             const QString &newFirstName,
                                             const QString &newMiddleName,
                                             const QString &newLastName,
                                             const QString &newPhone,
                                             const QString &newEmail,
                                             QWidget *parent)
    : ThemeableDialog(parent)
{
    setWindowTitle("Duplicate Client Detected");
    setFixedSize(520, 420);
    setWindowFlags(Qt::FramelessWindowHint | Qt::Dialog);
    setWindowOpacity(0.0);

    // Title
    titleLabel = new QLabel("A client with the same details already exists!");
    titleLabel->setWordWrap(true);

    // Existing client info
    existingLabel = new QLabel(
        QString("<b>Existing:</b><br>First: %1<br>Middle: %2<br>Last: %3<br>Phone: %4<br>Email: %5")
            .arg(existingFirstName, existingMiddleName, existingLastName, existingPhone, existingEmail)
        );
    existingLabel->setWordWrap(true);

    // New entry label
    newEntryLabel = new QLabel("New Entry (edit as needed):");

    // Editable fields for new entry
    editFirstName = new QLineEdit(newFirstName);
    editMiddleName = new QLineEdit(newMiddleName);
    editLastName = new QLineEdit(newLastName);
    editPhone = new QLineEdit(newPhone);
    editEmail = new QLineEdit(newEmail);

    QFormLayout *editForm = new QFormLayout();
    editForm->addRow("First Name:", editFirstName);
    editForm->addRow("Middle Name:", editMiddleName);
    editForm->addRow("Last Name:", editLastName);
    editForm->addRow("Phone:", editPhone);
    editForm->addRow("Email:", editEmail);

    // Highlight duplicates with pulsing glow
    auto addPulsingGlow = [](QLineEdit *field, bool duplicate) {
        if (duplicate) {
            QGraphicsDropShadowEffect *glow = new QGraphicsDropShadowEffect(field);
            glow->setBlurRadius(20);
            glow->setOffset(0);
            glow->setColor(QColor(255, 0, 0, 150));
            field->setGraphicsEffect(glow);

            QPropertyAnimation *anim = new QPropertyAnimation(glow, "color");
            anim->setDuration(1000);
            anim->setStartValue(QColor(255, 0, 0, 100));
            anim->setEndValue(QColor(255, 0, 0, 200));
            anim->setLoopCount(-1);
            anim->setEasingCurve(QEasingCurve::SineCurve);
            anim->start(QAbstractAnimation::DeleteWhenStopped);
        }
    };

    addPulsingGlow(editFirstName, editFirstName->text().trimmed() == existingFirstName);
    addPulsingGlow(editMiddleName, editMiddleName->text().trimmed() == existingMiddleName);
    addPulsingGlow(editLastName, editLastName->text().trimmed() == existingLastName);
    addPulsingGlow(editPhone, editPhone->text().trimmed() == existingPhone);
    addPulsingGlow(editEmail, editEmail->text().trimmed() == existingEmail);

    // Buttons
    proceedBtn = new QPushButton("Proceed Anyway");
    cancelBtn = new QPushButton("Cancel");

    // Layout
    QVBoxLayout *mainLayout = new QVBoxLayout(this);
    mainLayout->setContentsMargins(20, 20, 20, 20);
    mainLayout->setSpacing(10);
    mainLayout->addWidget(titleLabel);
    mainLayout->addWidget(existingLabel);
    mainLayout->addWidget(newEntryLabel);
    mainLayout->addLayout(editForm);

    QHBoxLayout *btnLayout = new QHBoxLayout();
    btnLayout->addStretch();
    btnLayout->addWidget(proceedBtn);
    btnLayout->addWidget(cancelBtn);
    mainLayout->addLayout(btnLayout);

    setLayout(mainLayout);

    // Glow effect on dialog
    QGraphicsDropShadowEffect *dialogGlow = new QGraphicsDropShadowEffect(this);
    dialogGlow->setBlurRadius(30);
    dialogGlow->setOffset(0, 0);
    setGraphicsEffect(dialogGlow);

    fadeIn();

    connect(proceedBtn, &QPushButton::clicked, this, &DuplicateClientDialog::onProceed);
    connect(cancelBtn, &QPushButton::clicked, this, &DuplicateClientDialog::onCancel);
}

void DuplicateClientDialog::updateTheme()
{
    Theme t = ThemeManager::instance()->currentTheme();
    setStyleSheet(QString("background-color:%1; border-radius:12px;").arg(t.background.name()));
    QGraphicsDropShadowEffect *dialogGlow = static_cast<QGraphicsDropShadowEffect*>(graphicsEffect());
    if (dialogGlow) {
        dialogGlow->setColor(t.accent);
    }

    titleLabel->setStyleSheet(QString("font-weight: bold; font-size: 14pt; color: %1;").arg(t.text.name()));
    existingLabel->setStyleSheet(QString("color: %1; font-size: 11pt;").arg(t.text.name()));
    newEntryLabel->setStyleSheet(QString("font-weight: bold; font-size: 12pt; color: %1;").arg(t.text.name()));

    QString fieldStyle =
        QString("QLineEdit { padding: 6px; border-radius: 6px; border: 2px solid %1; background-color: %2; color: %3; } "
        "QLineEdit:focus { border: 2px solid %4; }")
        .arg(t.primary.name())
        .arg(t.background.darker(115).name())
        .arg(t.text.name())
        .arg(t.accent.name());

    editFirstName->setStyleSheet(fieldStyle);
    editMiddleName->setStyleSheet(fieldStyle);
    editLastName->setStyleSheet(fieldStyle);
    editPhone->setStyleSheet(fieldStyle);
    editEmail->setStyleSheet(fieldStyle);

    proceedBtn->setStyleSheet(
        QString("QPushButton { background-color: %1; color: %2; border-radius: 8px; padding: 6px 14px; font-weight: bold; border: 2px solid %3; }"
        "QPushButton:hover { background-color: %4; border: 2px solid %5; }")
        .arg(t.primary.name())
        .arg(t.text.name())
        .arg(t.primary.lighter(120).name())
        .arg(t.primary.darker(120).name())
        .arg(t.accent.name()));

    cancelBtn->setStyleSheet(
        QString("QPushButton { background-color: %1; color: %2; border-radius: 8px; padding: 6px 14px; border: 2px solid %3; }"
        "QPushButton:hover { background-color: %4; border: 2px solid %5; }")
        .arg(t.secondary.name())
        .arg(t.text.name())
        .arg(t.secondary.lighter(120).name())
        .arg(t.secondary.darker(120).name())
        .arg(t.accent.name()));
}

// Fade animations
void DuplicateClientDialog::fadeIn() {
    fadeAnimation = new QPropertyAnimation(this, "windowOpacity");
    fadeAnimation->setDuration(250);
    fadeAnimation->setStartValue(0.0);
    fadeAnimation->setEndValue(1.0);
    fadeAnimation->start(QAbstractAnimation::DeleteWhenStopped);
}

void DuplicateClientDialog::fadeOut() {
    fadeAnimation = new QPropertyAnimation(this, "windowOpacity");
    fadeAnimation->setDuration(200);
    fadeAnimation->setStartValue(1.0);
    fadeAnimation->setEndValue(0.0);
    connect(fadeAnimation, &QPropertyAnimation::finished, this, &QDialog::close);
    fadeAnimation->start();
}

// Drag events
void DuplicateClientDialog::mousePressEvent(QMouseEvent *event) {
    if(event->button() == Qt::LeftButton) {
        mousePressed = true;
        mousePressPos = event->globalPosition().toPoint() - frameGeometry().topLeft();
    }
}

void DuplicateClientDialog::mouseMoveEvent(QMouseEvent *event) {
    if(mousePressed)
        move(event->globalPosition().toPoint() - mousePressPos);
}

void DuplicateClientDialog::mouseReleaseEvent(QMouseEvent *event) {
    Q_UNUSED(event);
    mousePressed = false;
}

// Slots
void DuplicateClientDialog::onProceed() {
    m_proceed = true;
    fadeOut();
    accept();
}

void DuplicateClientDialog::onCancel() {
    m_proceed = false;
    fadeOut();
    reject();
}

// Getters
QString DuplicateClientDialog::getEditedFirstName() const { return editFirstName->text(); }
QString DuplicateClientDialog::getEditedMiddleName() const { return editMiddleName->text(); }
QString DuplicateClientDialog::getEditedLastName() const { return editLastName->text(); }
QString DuplicateClientDialog::getEditedPhone() const { return editPhone->text(); }
QString DuplicateClientDialog::getEditedEmail() const { return editEmail->text(); }
