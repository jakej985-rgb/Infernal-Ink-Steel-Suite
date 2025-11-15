#pragma once

#include "themes/themeablewidget.h"
#include "appt/appointmenttableview.h"
#include "db/appointmentdb.h"

class CompletedAppointmentsTab : public ThemeableWidget {
    Q_OBJECT
public:
    explicit CompletedAppointmentsTab(AppointmentDB* db, QWidget* parent = nullptr);
    void loadCompletedAppointments();
    AppointmentTableView* getTableView() const { return tableView; }

private:
    AppointmentTableView* tableView;
    AppointmentDB* m_db;

    void setupUI();
};
