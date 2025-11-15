#pragma once

#include <QAbstractTableModel>
#include "db/appointmentdb.h"
#include "domain/appointment.h"

class AppointmentTableModel : public QAbstractTableModel {
    Q_OBJECT

public:
    explicit AppointmentTableModel(AppointmentDB* db, QObject *parent = nullptr);

    int rowCount(const QModelIndex &parent = QModelIndex()) const override;
    int columnCount(const QModelIndex &parent = QModelIndex()) const override;
    QVariant data(const QModelIndex &index, int role = Qt::DisplayRole) const override;
    QVariant headerData(int section, Qt::Orientation orientation, int role = Qt::DisplayRole) const override;

    void clear();
    void setAppointments(const QList<Appointment>& appointments);
    const QList<Appointment>& getAppointments() const { return m_appointments; }

private:
    AppointmentDB* m_db;
    QList<Appointment> m_appointments;
};
