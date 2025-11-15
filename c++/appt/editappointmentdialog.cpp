#include "editappointmentdialog.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QLabel>
#include <QColorDialog>
#include <QMessageBox>
#include <QGraphicsDropShadowEffect>
#include "helper/gloweffect.h"
#include "helper/thememanager.h"

EditAppointmentDialog::EditAppointmentDialog(Appointment &appt, QWidget *parent)
    : ThemeableDialog(parent), m_appt(appt)
{
    setWindowFlags(Qt::FramelessWindowHint | Qt::Dialog);
    setWindowOpacity(0.0);
    setFixedSize(400, 500);

    setupUI();

    // Fade in
    fadeAnimation = new QPropertyAnimation(this, "windowOpacity");
    fadeAnimation->setDuration(250);
    fadeAnimation->setStartValue(0.0);
    fadeAnimation->setEndValue(1.0);
    fadeAnimation->start(QAbstractAnimation::DeleteWhenStopped);
}

void EditAppointmentDialog::setupUI()
{
    QVBoxLayout *layout = new QVBoxLayout(this);

    clientNameEdit = new QLineEdit(m_appt.clientName);
    serviceEdit = new QLineEdit(m_appt.serviceType);

    serviceCategoryCombo = new QComboBox;
    serviceCategoryCombo->addItems({"Tattoo", "Piercing"});
    serviceCategoryCombo->setCurrentText(m_appt.serviceCategory);

    pricingCombo = new QComboBox;
    pricingCombo->addItems({"Regular", "Promo"});
    pricingCombo->setCurrentText(m_appt.priceType);

    priceChargedEdit = new QLineEdit(QString::number(m_appt.priceCharged));

    dateTimeEdit = new QDateTimeEdit(m_appt.dateTime);
    durationSpin = new QSpinBox;
    durationSpin->setRange(1, 480);
    durationSpin->setValue(m_appt.durationMinutes);

    notesEdit = new QTextEdit(m_appt.notes);
    colorBtn = new QPushButton("Choose Color");

    okBtn = new QPushButton("OK");
    cancelBtn = new QPushButton("Cancel");

    layout->addWidget(new QLabel("Client Name:")); layout->addWidget(clientNameEdit);
    layout->addWidget(new QLabel("Service Type:")); layout->addWidget(serviceEdit);
    layout->addWidget(new QLabel("Service Category:")); layout->addWidget(serviceCategoryCombo);
    layout->addWidget(new QLabel("Price Type:")); layout->addWidget(pricingCombo);
    layout->addWidget(new QLabel("Price Charged:")); layout->addWidget(priceChargedEdit);
    layout->addWidget(new QLabel("Date/Time:")); layout->addWidget(dateTimeEdit);
    layout->addWidget(new QLabel("Duration (minutes):")); layout->addWidget(durationSpin);
    layout->addWidget(new QLabel("Notes:")); layout->addWidget(notesEdit);
    layout->addWidget(colorBtn);

    QHBoxLayout *btnLayout = new QHBoxLayout;
    btnLayout->addWidget(okBtn);
    btnLayout->addWidget(cancelBtn);
    layout->addLayout(btnLayout);

    connect(colorBtn, &QPushButton::clicked, this, &EditAppointmentDialog::chooseColor);
    connect(okBtn, &QPushButton::clicked, this, &EditAppointmentDialog::accept);
    connect(cancelBtn, &QPushButton::clicked, this, &EditAppointmentDialog::reject);
}

void EditAppointmentDialog::updateTheme()
{
    auto theme = ThemeManager::instance()->currentTheme();
    QString fieldStyle = QString(
        "QLineEdit, QComboBox, QTextEdit, QDateTimeEdit, QSpinBox {"
        "  padding: 6px;"
        "  border-radius: 6px;"
        "  border: 2px solid %1;"
        "  background-color: %2;"
        "  color: %3;"
        "}"
        "QLineEdit:focus, QComboBox:focus, QTextEdit:focus, QDateTimeEdit:focus, QSpinBox:focus {"
        "  border: 2px solid %4;"
        "}"
    ).arg(theme.border.name(),
         theme.background.name(),
         theme.text.name(),
         theme.accent.name());

    clientNameEdit->setStyleSheet(fieldStyle);
    serviceEdit->setStyleSheet(fieldStyle);
    serviceCategoryCombo->setStyleSheet(fieldStyle);
    pricingCombo->setStyleSheet(fieldStyle);
    priceChargedEdit->setStyleSheet(fieldStyle);
    notesEdit->setStyleSheet(fieldStyle);
    dateTimeEdit->setStyleSheet(fieldStyle);
    durationSpin->setStyleSheet(fieldStyle);

    styleButton(colorBtn);
    styleButton(okBtn);
    styleButton(cancelBtn);

    // Dialog glow
    QGraphicsDropShadowEffect *dialogGlow = new QGraphicsDropShadowEffect(this);
    dialogGlow->setBlurRadius(30);
    dialogGlow->setOffset(0,0);
    dialogGlow->setColor(theme.accent);
    setGraphicsEffect(dialogGlow);
}

void EditAppointmentDialog::chooseColor()
{
    QColor chosen = QColorDialog::getColor(m_appt.color, this, "Select Color");
    if(chosen.isValid()) m_appt.color = chosen;
}

void EditAppointmentDialog::accept()
{
    if(clientNameEdit->text().isEmpty() || serviceEdit->text().isEmpty())
    {
        QMessageBox::warning(this, "Validation Error", "Client name and service type cannot be empty.");
        return;
    }

    m_appt.clientName = clientNameEdit->text();
    m_appt.serviceType = serviceEdit->text();
    m_appt.serviceCategory = serviceCategoryCombo->currentText();
    m_appt.priceType = pricingCombo->currentText();
    m_appt.priceCharged = priceChargedEdit->text().toDouble();
    m_appt.dateTime = dateTimeEdit->dateTime();
    m_appt.durationMinutes = durationSpin->value();
    m_appt.notes = notesEdit->toPlainText();

    QDialog::accept();
}