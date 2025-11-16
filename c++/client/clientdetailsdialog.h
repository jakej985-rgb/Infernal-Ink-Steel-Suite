#pragma once
#include "themes/themeabledialog.h"
#include <QLabel>
#include <QTableWidget>
#include <QPushButton>

class Appointment;
class AppointmentDB;

class ClientDetailsDialog : public ThemeableDialog {
    Q_OBJECT
public:
    explicit ClientDetailsDialog(int clientId,
                                 const QString &firstName,
                                 const QString &middleName,
                                 const QString &lastName,
                                 const QString &phone,
                                 const QString &email,
                                 int visits,
                                 AppointmentDB *appointmentDb,
                                 QWidget *parent = nullptr);

    int clientId() const { return m_clientId; }

private:
    int m_clientId;
    int m_initialVisits;
    AppointmentDB *m_appointmentDb;

    QLabel *idLabel;
    QLabel *nameLabel;
    QLabel *emailLabel;
    QLabel *phoneLabel;
    QLabel *visitsLabel;

    QTableWidget *historyTable;
    QPushButton *closeButton;

    void updateTheme() override;
    void populateHistory();
    void showEmptyHistoryPlaceholder();
    QString formatDateWithStatus(const Appointment &appt) const;
};
