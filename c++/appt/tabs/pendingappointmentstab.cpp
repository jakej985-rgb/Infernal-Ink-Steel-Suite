#include "pendingappointmentstab.h"
#include <QVBoxLayout>
#include "domain/appointment.h"

PendingAppointmentsTab::PendingAppointmentsTab(AppointmentDB* db, QWidget* parent)
    : ThemeableWidget(parent), m_db(db) {
    setupUI();
    loadPendingAppointments();
}

void PendingAppointmentsTab::setupUI() {
    tableView = new AppointmentTableView(m_db, this);
    QVBoxLayout* layout = new QVBoxLayout(this);
    layout->addWidget(tableView);
    setLayout(layout);
}

void PendingAppointmentsTab::loadPendingAppointments() {
    QList<Appointment> pendingAppointments = m_db->getAppointmentsByStatus("Scheduled");
    tableView->getModel()->setAppointments(pendingAppointments);
}
