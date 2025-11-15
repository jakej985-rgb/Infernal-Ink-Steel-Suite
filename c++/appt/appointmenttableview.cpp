#include "appointmenttableview.h"
#include "helper/thememanager.h"
#include <QHeaderView>

AppointmentTableView::AppointmentTableView(AppointmentDB* db, QWidget *parent)
    : QTableView(parent)
{
    m_model = new AppointmentTableModel(db, this);
    setModel(m_model);

    setSelectionBehavior(QAbstractItemView::SelectRows);
    setSelectionMode(QAbstractItemView::SingleSelection);
    setAlternatingRowColors(true);
    horizontalHeader()->setStretchLastSection(true);

}

void AppointmentTableView::clear() {
    m_model->clear();
}
