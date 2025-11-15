#include "calendartab.h"
#include <QVBoxLayout>

CalendarTab::CalendarTab(AppointmentDB* db, QWidget* parent)
    : ThemeableWidget(parent), m_db(db) {
    setupUI();
    setupConnections();
}

void CalendarTab::setupUI() {
    calendarWidget = new ThemeableCalendarWidget(this);
    dailyView = new DailyTableWidget(m_db, this);
    weekView = new WeekTableWidget(m_db, this);
    monthView = new MonthTableWidget(m_db, this);

    tabWidget = new QTabWidget(this);
    tabWidget->addTab(dailyView, "Day");
    tabWidget->addTab(weekView, "Week");
    tabWidget->addTab(monthView, "Month");

    QVBoxLayout* layout = new QVBoxLayout(this);
    layout->addWidget(calendarWidget);
    layout->addWidget(tabWidget);
    setLayout(layout);
}

void CalendarTab::setupConnections() {
    connect(calendarWidget->calendar(), &QCalendarWidget::selectionChanged, this, [this]() {
        QDate selectedDate = calendarWidget->calendar()->selectedDate();
        dailyView->setDate(selectedDate);
        weekView->setDate(selectedDate);
        monthView->setDate(selectedDate);
    });
}

void CalendarTab::refreshCalendarView() {
    QDate selectedDate = calendarWidget->calendar()->selectedDate();
    dailyView->setDate(selectedDate);
    weekView->setDate(selectedDate);
    monthView->setDate(selectedDate);
}
