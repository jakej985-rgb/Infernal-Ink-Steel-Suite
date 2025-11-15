#ifndef APPOINTMENTACTIONDIALOG_H
#define APPOINTMENTACTIONDIALOG_H

#include <QDialog>
#include <QPushButton>
#include <QDateTimeEdit>
#include <QSpinBox>
#include <QLineEdit>
#include <QTextEdit>
#include <QComboBox>
#include <QCalendarWidget>
#include "domain/appointment.h"
#include "themes/themeabledialog.h"

class AppointmentActionDialog : public ThemeableDialog
{
    Q_OBJECT
public:
    explicit AppointmentActionDialog(Appointment &appt, QWidget *parent = nullptr);

    enum ActionType { None, Completed, Rescheduled, Canceled, NoShow };
    ActionType action() const { return m_action; }

private slots:
    void onCompleteClicked();
    void onRescheduleClicked();
    void onCancelClicked();
    void onNoShowClicked();
    void onOkClicked();
    void onCancelEditClicked();

private:
    void updateTheme() override;
    void setupUI();
    void showCompleteWidget();
    void showRescheduleWidget();
    void resetWidgets();

    Appointment &m_appt;
    ActionType m_action = None;

    // Buttons
    QPushButton *completeBtn;
    QPushButton *rescheduleBtn;
    QPushButton *cancelBtn;
    QPushButton *noShowBtn;
    QPushButton *okBtn;
    QPushButton *cancelEditBtn;

    // Complete section
    QWidget *completeWidget;
    QLineEdit *serviceEdit;
    QComboBox *serviceCategoryCombo;
    QComboBox *pricingCombo;
    QLineEdit *priceChargedEdit;
    QSpinBox *durationSpin;
    QTextEdit *notesEdit;

    // Reschedule section
    QWidget *rescheduleWidget;
    QDateTimeEdit *rescheduleDateTime;
};

#endif //APPOINTMENTACTIONDIALOG_H
