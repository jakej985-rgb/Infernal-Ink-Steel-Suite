#include "completedappointmentstab.h"
#include <QVBoxLayout>
#include "domain/appointment.h"

CompletedAppointmentsTab::CompletedAppointmentsTab(AppointmentDB* db, QWidget* parent)
    : ThemeableWidget(parent), m_db(db) {
    setupUI();
    loadCompletedAppointments();
}

void CompletedAppointmentsTab::setupUI() {
    tableView = new AppointmentTableView(m_db, this);
    QVBoxLayout* layout = new QVBoxLayout(this);
    layout->addWidget(tableView);
    setLayout(layout);
}

void CompletedAppointmentsTab::loadCompletedAppointments() {
    QList<Appointment> completedAppointments = m_db->getAppointmentsByStatus("Completed");
    tableView->getModel()->setAppointments(completedAppointments);
}
