#include "upcomingappointmentstab.h"
#include <QVBoxLayout>
#include <QDateTime>
#include "domain/appointment.h"

UpcomingAppointmentsTab::UpcomingAppointmentsTab(AppointmentDB* db, QWidget* parent)
    : ThemeableWidget(parent), m_db(db) {
    setupUI();
    loadUpcomingAppointments();
}

void UpcomingAppointmentsTab::setupUI() {
    tableView = new AppointmentTableView(m_db, this);
    QVBoxLayout* layout = new QVBoxLayout(this);
    layout->addWidget(tableView);
    setLayout(layout);
}

void UpcomingAppointmentsTab::loadUpcomingAppointments() {
    // Logic to get appointments from 1 hour ago to the end of the day
    QDateTime now = QDateTime::currentDateTime();
    QDateTime start = now.addSecs(-3600); // 1 hour ago
    QDateTime end = QDateTime(now.date(), QTime(23, 59, 59));

    QList<Appointment> upcomingAppointments = m_db->getAppointmentsBetween(start, end);
    tableView->getModel()->setAppointments(upcomingAppointments);
}
