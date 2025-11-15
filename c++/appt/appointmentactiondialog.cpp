#include "appointmentactiondialog.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QLabel>
#include <QGraphicsDropShadowEffect>
#include <QPropertyAnimation>
#include <QMessageBox>
#include "helper/gloweffect.h"
#include "helper/thememanager.h"

AppointmentActionDialog::AppointmentActionDialog(Appointment &appt, QWidget *parent)
    : ThemeableDialog(parent), m_appt(appt)
{
    setWindowFlags(Qt::FramelessWindowHint | Qt::Dialog);
    setWindowOpacity(0.0);
    setFixedSize(450, 500);

    setupUI();

    // Fade in
    QPropertyAnimation *fadeIn = new QPropertyAnimation(this, "windowOpacity");
    fadeIn->setDuration(250);
    fadeIn->setStartValue(0.0);
    fadeIn->setEndValue(1.0);
    fadeIn->start(QAbstractAnimation::DeleteWhenStopped);
}

void AppointmentActionDialog::setupUI()
{
    QVBoxLayout *mainLayout = new QVBoxLayout(this);
    mainLayout->setSpacing(10);
    mainLayout->setContentsMargins(20, 20, 20, 20);

    QLabel *titleLabel = new QLabel("Select Action for Appointment:");
    titleLabel->setStyleSheet("font-weight: bold; font-size: 14pt; color: white;");
    mainLayout->addWidget(titleLabel);

    QHBoxLayout *actionLayout = new QHBoxLayout;
    completeBtn = new QPushButton("Complete");
    rescheduleBtn = new QPushButton("Reschedule");
    cancelBtn = new QPushButton("Cancel");
    noShowBtn = new QPushButton("No Show");
    actionLayout->addWidget(completeBtn);
    actionLayout->addWidget(rescheduleBtn);
    actionLayout->addWidget(cancelBtn);
    actionLayout->addWidget(noShowBtn);
    mainLayout->addLayout(actionLayout);

    // Complete widget
    completeWidget = new QWidget(this);
    QVBoxLayout *completeLayout = new QVBoxLayout(completeWidget);

    serviceEdit = new QLineEdit(m_appt.serviceType);
    serviceCategoryCombo = new QComboBox; serviceCategoryCombo->addItems({"Tattoo","Piercing"});
    serviceCategoryCombo->setCurrentText(m_appt.serviceCategory);
    pricingCombo = new QComboBox; pricingCombo->addItems({"Regular","Promo"});
    pricingCombo->setCurrentText(m_appt.priceType);
    priceChargedEdit = new QLineEdit(QString::number(m_appt.priceCharged));
    durationSpin = new QSpinBox; durationSpin->setRange(1,480); durationSpin->setValue(m_appt.durationMinutes);
    notesEdit = new QTextEdit(m_appt.notes);

    completeLayout->addWidget(new QLabel("Service Type:")); completeLayout->addWidget(serviceEdit);
    completeLayout->addWidget(new QLabel("Service Category:")); completeLayout->addWidget(serviceCategoryCombo);
    completeLayout->addWidget(new QLabel("Pricing:")); completeLayout->addWidget(pricingCombo);
    completeLayout->addWidget(new QLabel("Price Charged:")); completeLayout->addWidget(priceChargedEdit);
    completeLayout->addWidget(new QLabel("Duration (minutes):")); completeLayout->addWidget(durationSpin);
    completeLayout->addWidget(new QLabel("Notes:")); completeLayout->addWidget(notesEdit);

    QHBoxLayout *completeBtnLayout = new QHBoxLayout;
    okBtn = new QPushButton("OK"); cancelEditBtn = new QPushButton("Cancel");
    completeBtnLayout->addWidget(okBtn);
    completeBtnLayout->addWidget(cancelEditBtn);
    completeLayout->addLayout(completeBtnLayout);

    mainLayout->addWidget(completeWidget);
    completeWidget->hide();

    // Reschedule widget
    rescheduleWidget = new QWidget(this);
    QVBoxLayout *rescheduleLayout = new QVBoxLayout(rescheduleWidget);
    rescheduleDateTime = new QDateTimeEdit(m_appt.dateTime);
    rescheduleLayout->addWidget(new QLabel("Pick new Date/Time:"));
    rescheduleLayout->addWidget(rescheduleDateTime);

    QHBoxLayout *rescheduleBtnLayout = new QHBoxLayout;
    QPushButton *okReschedule = new QPushButton("OK");
    QPushButton *cancelReschedule = new QPushButton("Cancel");
    rescheduleBtnLayout->addWidget(okReschedule);
    rescheduleBtnLayout->addWidget(cancelReschedule);
    rescheduleLayout->addLayout(rescheduleBtnLayout);

    mainLayout->addWidget(rescheduleWidget);
    rescheduleWidget->hide();

    // Connections
    connect(completeBtn, &QPushButton::clicked, this, &AppointmentActionDialog::onCompleteClicked);
    connect(rescheduleBtn, &QPushButton::clicked, this, &AppointmentActionDialog::onRescheduleClicked);
    connect(cancelBtn, &QPushButton::clicked, this, &AppointmentActionDialog::onCancelClicked);
    connect(noShowBtn, &QPushButton::clicked, this, &AppointmentActionDialog::onNoShowClicked);

    connect(okBtn, &QPushButton::clicked, this, &AppointmentActionDialog::onOkClicked);
    connect(cancelEditBtn, &QPushButton::clicked, this, &AppointmentActionDialog::onCancelEditClicked);
    connect(okReschedule, &QPushButton::clicked, this, [=](){
        m_appt.dateTime = rescheduleDateTime->dateTime();
        m_action = Rescheduled;
        this->accept();
    });
    connect(cancelReschedule, &QPushButton::clicked, this, &AppointmentActionDialog::resetWidgets);
}

void AppointmentActionDialog::updateTheme()
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

    serviceEdit->setStyleSheet(fieldStyle);
    serviceCategoryCombo->setStyleSheet(fieldStyle);
    pricingCombo->setStyleSheet(fieldStyle);
    priceChargedEdit->setStyleSheet(fieldStyle);
    durationSpin->setStyleSheet(fieldStyle);
    notesEdit->setStyleSheet(fieldStyle);
    rescheduleDateTime->setStyleSheet(fieldStyle);

    styleButton(completeBtn);
    styleButton(rescheduleBtn);
    styleButton(cancelBtn);
    styleButton(noShowBtn);
    styleButton(okBtn);
    styleButton(cancelEditBtn);
}

void AppointmentActionDialog::showCompleteWidget()
{
    resetWidgets();
    completeWidget->show();
}

void AppointmentActionDialog::showRescheduleWidget()
{
    resetWidgets();
    rescheduleWidget->show();
}

void AppointmentActionDialog::resetWidgets()
{
    completeWidget->hide();
    rescheduleWidget->hide();
}

void AppointmentActionDialog::onCompleteClicked() { showCompleteWidget(); }
void AppointmentActionDialog::onRescheduleClicked() { showRescheduleWidget(); }
void AppointmentActionDialog::onCancelClicked() { m_action = Canceled; this->accept(); }
void AppointmentActionDialog::onNoShowClicked() { m_action = NoShow; this->accept(); }
void AppointmentActionDialog::onOkClicked()
{
    if(serviceEdit->text().isEmpty())
    {
        QMessageBox::warning(this, "Validation Error", "Service type cannot be empty.");
        return;
    }

    m_appt.serviceType = serviceEdit->text();
    m_appt.serviceCategory = serviceCategoryCombo->currentText();
    m_appt.priceType = pricingCombo->currentText();
    m_appt.priceCharged = priceChargedEdit->text().toDouble();
    m_appt.durationMinutes = durationSpin->value();
    m_appt.notes = notesEdit->toPlainText();

    m_action = Completed;
    this->accept();
}

void AppointmentActionDialog::onCancelEditClicked() { resetWidgets(); }