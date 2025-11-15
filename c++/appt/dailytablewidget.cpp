#include "dailytablewidget.h"
#include "helper/thememanager.h"
#include <QHeaderView>
#include <QStandardItemModel>

class DailyAppointmentModel : public QStandardItemModel {
public:
    DailyAppointmentModel(AppointmentDB* db, const QDate& date, QObject* parent = nullptr)
        : QStandardItemModel(parent), m_db(db), m_date(date) {
        setHorizontalHeaderLabels({"Time", "Client", "Service"});
        loadData();
    }

    void loadData() {
        removeRows(0, rowCount());
        auto appointments = m_db->getAppointmentsByDate(m_date);
        for (const auto& appt : appointments) {
            QList<QStandardItem*> row;
            row.append(new QStandardItem(appt.dateTime.time().toString("HH:mm")));
            row.append(new QStandardItem(appt.clientName));
            row.append(new QStandardItem(appt.serviceType));
            appendRow(row);
        }
    }

private:
    AppointmentDB* m_db;
    QDate m_date;
};

#include <QVBoxLayout>

DailyTableWidget::DailyTableWidget(AppointmentDB* db, QWidget *parent)
    : ThemeableWidget(parent), m_db(db)
{
    m_tableView = new QTableView(this);
    m_date = QDate::currentDate();
    m_tableView->setModel(new DailyAppointmentModel(m_db, m_date, this));
    m_tableView->horizontalHeader()->setStretchLastSection(true);

    auto* layout = new QVBoxLayout(this);
    layout->setContentsMargins(0,0,0,0);
    layout->addWidget(m_tableView);
    setLayout(layout);
}

void DailyTableWidget::setDate(const QDate& date) {
    m_date = date;
    m_tableView->setModel(new DailyAppointmentModel(m_db, m_date, this));
}

void DailyTableWidget::updateTheme() {
    // The base class handles theme propagation.
    // The global stylesheet in styles.qss.template
    // should be used to style QTableView.
}
