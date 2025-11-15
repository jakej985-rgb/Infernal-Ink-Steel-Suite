#ifndef APPOINTMENTDIALOG_H
#define APPOINTMENTDIALOG_H

#include <QPushButton>
#include <QComboBox>
#include <QDateTimeEdit>
#include <QPlainTextEdit>
#include <QSqlDatabase>
#include "domain/appointment.h"
#include "db/clientdb.h"
#include "db/appointmentdb.h"
#include "db/userdb.h"
#include "client/clientdialog.h"  // <- new Add Client dialog
#include "themes/themeabledialog.h"

class AppointmentDialog : public ThemeableDialog
{
    Q_OBJECT

public:
    explicit AppointmentDialog(QSqlDatabase* db, int currentUserId, bool isAdmin, QWidget *parent = nullptr);
    void setAppointment(const Appointment &appt);

private slots:
    void saveAppointment();
    void openAddClientDialog();

private:
    void setupUi();
    void loadClients();

    QSqlDatabase* m_db;
    int m_currentUserId;
    bool m_isAdmin;

    Appointment m_appointment;

    QPushButton *saveButton;
    QPushButton *cancelButton;
    QPushButton *addClientButton;

    QComboBox *clientComboBox;
    QDateTimeEdit *dateTimeEdit;
    QPlainTextEdit *notesEdit;
};

#endif // APPOINTMENTDIALOG_H
