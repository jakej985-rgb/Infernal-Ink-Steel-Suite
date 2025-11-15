#pragma once

#include <QTableView>
#include "appointmenttablemodel.h"
#include <QTableView>

class AppointmentTableView : public QTableView {
    Q_OBJECT

public:
    explicit AppointmentTableView(AppointmentDB* db, QWidget *parent = nullptr);

    void clear();
    void updateTheme();
    AppointmentTableModel* getModel() const { return m_model; }

private:
    AppointmentTableModel* m_model;
};
