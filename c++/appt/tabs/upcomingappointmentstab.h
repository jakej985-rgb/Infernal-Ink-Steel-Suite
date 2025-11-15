#pragma once

#include "themes/themeablewidget.h"
#include "appt/appointmenttableview.h"
#include "db/appointmentdb.h"

class UpcomingAppointmentsTab : public ThemeableWidget {
    Q_OBJECT
public:
    explicit UpcomingAppointmentsTab(AppointmentDB* db, QWidget* parent = nullptr);
    void loadUpcomingAppointments();
    AppointmentTableView* getTableView() const { return tableView; }

private:
    AppointmentTableView* tableView;
    AppointmentDB* m_db;

    void setupUI();
};
