#include "appointmentpage.h"
#include <QMessageBox>
#include <QHeaderView>
#include "db/databasemanager.h"
#include "helper/modernbutton.h"
#include "appt/appointmentdialog.h"
#include "appt/editappointmentdialog.h"
#include "appt/appointmentactiondialog.h"

AppointmentsPage::AppointmentsPage(AppointmentDB* db, int currentUserId, bool isAdmin, QWidget* parent)
    : ThemeableWidget(parent), m_db(db), m_currentUserId(currentUserId), m_isAdmin(isAdmin)
{
    setupUI();
    setupConnections();
}

void AppointmentsPage::setupUI() {
    mainTabWidget = new QTabWidget(this);
    upcomingTab = new UpcomingAppointmentsTab(m_db, this);
    pendingTab = new PendingAppointmentsTab(m_db, this);
    completedTab = new CompletedAppointmentsTab(m_db, this);
    calendarTab = new CalendarTab(m_db, this);

    mainTabWidget->addTab(upcomingTab, "Upcoming");
    mainTabWidget->addTab(pendingTab, "Pending");
    mainTabWidget->addTab(completedTab, "Completed");
    mainTabWidget->addTab(calendarTab, "Calendar");

    addButton = new ModernButton("Add", QIcon(":/icons/add.png"), this);
    editButton = new ModernButton("Edit", QIcon(":/icons/edit.png"), this);
    deleteButton = new ModernButton("Delete", QIcon(":/icons/delete.png"), this);
    refreshButton = new ModernButton("Refresh", QIcon(":/icons/refresh.png"), this);

    auto* buttonLayout = new QHBoxLayout;
    buttonLayout->addWidget(addButton);
    buttonLayout->addWidget(editButton);
    buttonLayout->addWidget(deleteButton);
    buttonLayout->addWidget(refreshButton);
    buttonLayout->addStretch();

    auto* mainLayout = new QVBoxLayout(this);
    mainLayout->addWidget(mainTabWidget);
    mainLayout->addLayout(buttonLayout);
    setLayout(mainLayout);

    updateButtonStates(0);
}

void AppointmentsPage::setupConnections() {
    connect(addButton, &QPushButton::clicked, this, &AppointmentsPage::addAppointment);
    connect(editButton, &QPushButton::clicked, this, &AppointmentsPage::editSelectedAppointment);
    connect(deleteButton, &QPushButton::clicked, this, &AppointmentsPage::deleteSelectedAppointment);
    connect(refreshButton, &QPushButton::clicked, this, &AppointmentsPage::refreshAppointments);
    connect(mainTabWidget, &QTabWidget::currentChanged, this, &AppointmentsPage::updateButtonStates);

    connect(upcomingTab->getTableView(), &QTableView::doubleClicked, this, &AppointmentsPage::handleAppointmentDoubleClick);
    connect(pendingTab->getTableView(), &QTableView::doubleClicked, this, &AppointmentsPage::handleAppointmentDoubleClick);
    connect(completedTab->getTableView(), &QTableView::doubleClicked, this, &AppointmentsPage::handleAppointmentDoubleClick);
}

void AppointmentsPage::handleAppointmentDoubleClick(const QModelIndex& index) {
    AppointmentTableView* tableView = qobject_cast<AppointmentTableView*>(sender());
    if (!tableView || !index.isValid()) return;

    int row = index.row();
    const QList<Appointment>& appts = tableView->getModel()->getAppointments();
    if (row < 0 || row >= appts.size()) return;

    Appointment appt = appts[row];
    AppointmentActionDialog dialog(appt, this);

    if (dialog.exec() == QDialog::Accepted) {
        if (!m_db->updateAppointment(appt))
            QMessageBox::warning(this, "Error", "Failed to update appointment.");
        refreshAppointments();
    }
}

void AppointmentsPage::updateButtonStates(int index) {
    bool isCalendarTab = mainTabWidget->widget(index) == calendarTab;
    addButton->setEnabled(!isCalendarTab);
    editButton->setEnabled(!isCalendarTab);
    deleteButton->setEnabled(!isCalendarTab);
}

AppointmentTableView* AppointmentsPage::getCurrentTableView() {
    QWidget* currentWidget = mainTabWidget->currentWidget();
    return currentWidget->findChild<AppointmentTableView*>();
}

void AppointmentsPage::addAppointment() {
    if (!m_db) return;
    AppointmentDialog dialog(m_db->database(), m_currentUserId, m_isAdmin, this);
    if (dialog.exec() == QDialog::Accepted)
        refreshAppointments();
}

void AppointmentsPage::editSelectedAppointment() {
    AppointmentTableView* tableView = getCurrentTableView();
    if (!tableView) return;

    auto selection = tableView->selectionModel()->selection();
    if (selection.isEmpty()) {
        QMessageBox::information(this, "Select", "Please select an appointment to edit.");
        return;
    }

    int row = selection.first().indexes().first().row();
    const QList<Appointment>& appts = tableView->getModel()->getAppointments();
    if (row < 0 || row >= appts.size()) return;

    Appointment appt = appts[row];
    EditAppointmentDialog dialog(appt, this);
    if (dialog.exec() == QDialog::Accepted) {
        if (!m_db->updateAppointment(appt))
            QMessageBox::warning(this, "Error", "Failed to update appointment.");
        refreshAppointments();
    }
}

void AppointmentsPage::deleteSelectedAppointment() {
    AppointmentTableView* tableView = getCurrentTableView();
    if (!tableView) return;

    auto selection = tableView->selectionModel()->selection();
    if (selection.isEmpty()) {
        QMessageBox::information(this, "Select", "Please select an appointment to delete.");
        return;
    }

    int row = selection.first().indexes().first().row();
    const QList<Appointment>& appts = tableView->getModel()->getAppointments();
    if (row < 0 || row >= appts.size()) return;

    int id = appts[row].id;
    if (!m_db->deleteAppointment(id))
        QMessageBox::warning(this, "Error", "Failed to delete appointment.");
    refreshAppointments();
}

void AppointmentsPage::refreshAppointments() {
    upcomingTab->loadUpcomingAppointments();
    pendingTab->loadPendingAppointments();
    completedTab->loadCompletedAppointments();
    calendarTab->refreshCalendarView();
}
