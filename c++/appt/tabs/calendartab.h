#pragma once

#include "themes/themeablewidget.h"
#include <QTabWidget>
#include "appt/themeablecalendarwidget.h"
#include "appt/dailytablewidget.h"
#include "appt/weektablewidget.h"
#include "appt/monthtablewidget.h"
#include "db/appointmentdb.h"

class CalendarTab : public ThemeableWidget {
    Q_OBJECT
public:
    explicit CalendarTab(AppointmentDB* db, QWidget* parent = nullptr);
    void refreshCalendarView();

private:
    AppointmentDB* m_db;
    ThemeableCalendarWidget* calendarWidget;
    QTabWidget* tabWidget;
    DailyTableWidget* dailyView;
    WeekTableWidget* weekView;
    MonthTableWidget* monthView;

    void setupUI();
    void setupConnections();
};
