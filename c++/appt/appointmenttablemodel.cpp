#include "appointmenttablemodel.h"

AppointmentTableModel::AppointmentTableModel(AppointmentDB* db, QObject *parent)
    : QAbstractTableModel(parent), m_db(db)
{
    clear();
}

int AppointmentTableModel::rowCount(const QModelIndex &parent) const {
    Q_UNUSED(parent);
    return m_appointments.size();
}

int AppointmentTableModel::columnCount(const QModelIndex &parent) const {
    Q_UNUSED(parent);
    return 6;
}

QVariant AppointmentTableModel::data(const QModelIndex &index, int role) const {
    if (!index.isValid() || role != Qt::DisplayRole) {
        return QVariant();
    }

    const Appointment &appointment = m_appointments.at(index.row());

    switch (index.column()) {
        case 0: return appointment.clientName;
        case 1: return appointment.userId;
        case 2: return appointment.dateTime.date().toString("yyyy-MM-dd");
        case 3: return appointment.dateTime.time().toString("HH:mm");
        case 4: return appointment.serviceType;
        case 5: return appointment.status;
        default: return QVariant();
    }
}

QVariant AppointmentTableModel::headerData(int section, Qt::Orientation orientation, int role) const {
    if (role != Qt::DisplayRole || orientation != Qt::Horizontal) {
        return QVariant();
    }

    switch (section) {
        case 0: return "Client";
        case 1: return "User";
        case 2: return "Date";
        case 3: return "Time";
        case 4: return "Service";
        case 5: return "Status";
        default: return QVariant();
    }
}

void AppointmentTableModel::clear() {
    beginResetModel();
    m_appointments.clear();
    endResetModel();
}

void AppointmentTableModel::setAppointments(const QList<Appointment>& appointments) {
    beginResetModel();
    m_appointments = appointments;
    endResetModel();
}
