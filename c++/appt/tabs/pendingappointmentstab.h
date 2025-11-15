#pragma once

#include "themes/themeablewidget.h"
#include "appt/appointmenttableview.h"
#include "db/appointmentdb.h"

class PendingAppointmentsTab : public ThemeableWidget {
    Q_OBJECT
public:
    explicit PendingAppointmentsTab(AppointmentDB* db, QWidget* parent = nullptr);
    void loadPendingAppointments();
    AppointmentTableView* getTableView() const { return tableView; }

private:
    AppointmentTableView* tableView;
    AppointmentDB* m_db;

    void setupUI();
};
