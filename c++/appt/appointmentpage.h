#pragma once
#include <QWidget>
#include <QLineEdit>
#include <QHBoxLayout>
#include <QVBoxLayout>
#include "appt/appointmenttableview.h"
#include "appt/appointmentdialog.h"
#include "appt/editappointmentdialog.h"
#include "appt/appointmentactiondialog.h"
#include "helper/modernbutton.h"
#include "db/appointmentdb.h"
#include "themes/themeablewidget.h"
#include "appt/tabs/upcomingappointmentstab.h"
#include "appt/tabs/pendingappointmentstab.h"
#include "appt/tabs/completedappointmentstab.h"
#include "appt/tabs/calendartab.h"
#include <QTabWidget>

class AppointmentsPage : public ThemeableWidget {
    Q_OBJECT
public:
    explicit AppointmentsPage(AppointmentDB* db, int currentUserId, bool isAdmin, QWidget* parent = nullptr);

private slots:
    void addAppointment();
    void editSelectedAppointment();
    void deleteSelectedAppointment();
    void refreshAppointments();
    void updateButtonStates(int index);
    void handleAppointmentDoubleClick(const QModelIndex& index);

private:
    AppointmentDB* m_db;
    int m_currentUserId;
    bool m_isAdmin;

    QTabWidget* mainTabWidget;
    UpcomingAppointmentsTab* upcomingTab;
    PendingAppointmentsTab* pendingTab;
    CompletedAppointmentsTab* completedTab;
    CalendarTab* calendarTab;

    ModernButton* addButton;
    ModernButton* editButton;
    ModernButton* deleteButton;
    ModernButton* refreshButton;

    void setupUI();
    void setupConnections();
    AppointmentTableView* getCurrentTableView();
};
