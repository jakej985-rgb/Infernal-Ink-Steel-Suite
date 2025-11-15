#ifndef EDITAPPOINTMENTDIALOG_H
#define EDITAPPOINTMENTDIALOG_H

#include <QDialog>
#include <QLineEdit>
#include <QComboBox>
#include <QDateTimeEdit>
#include <QSpinBox>
#include <QTextEdit>
#include <QPushButton>
#include <QPropertyAnimation>
#include "domain/appointment.h"
#include "themes/themeabledialog.h"

class EditAppointmentDialog : public ThemeableDialog
{
    Q_OBJECT
public:
    explicit EditAppointmentDialog(Appointment &appt, QWidget *parent = nullptr);

private:
    void updateTheme() override;
    Appointment &m_appt;

    QLineEdit *clientNameEdit;
    QLineEdit *serviceEdit;
    QComboBox *serviceCategoryCombo;
    QComboBox *pricingCombo;
    QLineEdit *priceChargedEdit;
    QDateTimeEdit *dateTimeEdit;
    QSpinBox *durationSpin;
    QTextEdit *notesEdit;
    QPushButton *colorBtn;
    QPushButton *okBtn;
    QPushButton *cancelBtn;

    QPropertyAnimation *fadeAnimation;

    void setupUI();
    void applyStyles();

private slots:
    void chooseColor();
    void accept() override;
};

#endif // EDITAPPOINTMENTDIALOG_H