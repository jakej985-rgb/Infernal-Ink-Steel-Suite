#include "weektablewidget.h"
#include "helper/thememanager.h"
#include <QHeaderView>
#include <QStandardItemModel>
#include <QDate>

class WeekAppointmentModel : public QStandardItemModel {
public:
    WeekAppointmentModel(AppointmentDB* db, const QDate& date, QObject* parent = nullptr)
        : QStandardItemModel(parent), m_db(db), m_date(date) {

        QStringList days;
        for (int i = 0; i < 7; ++i) {
            days << m_date.addDays(i).toString("ddd, MMM d");
        }
        setHorizontalHeaderLabels(days);

        // Set up the rows for the time slots
        setRowCount(18); // 9am to 5pm, half-hour slots
        QStringList timeSlots;
        for (int i = 0; i < 18; ++i) {
            QTime time = QTime(9, 0).addSecs(i * 1800);
            timeSlots << time.toString("h:mm ap");
        }
        setVerticalHeaderLabels(timeSlots);

        loadData();
    }

    void loadData() {
        for (int day = 0; day < 7; ++day) {
            QDate currentDate = m_date.addDays(day);
            auto appointments = m_db->getAppointmentsByDate(currentDate);
            for (const auto& appt : appointments) {
                QTime time = appt.dateTime.time();
                int row = (time.hour() - 9) * 2 + (time.minute() / 30);
                if (row >= 0 && row < 18) {
                    QStandardItem* item = new QStandardItem(appt.clientName);
                    setItem(row, day, item);
                }
            }
        }
    }

private:
    AppointmentDB* m_db;
    QDate m_date;
};

#include <QVBoxLayout>

WeekTableWidget::WeekTableWidget(AppointmentDB* db, QWidget *parent)
    : ThemeableWidget(parent), m_db(db)
{
    m_tableView = new QTableView(this);
    m_date = QDate::currentDate();
    m_tableView->setModel(new WeekAppointmentModel(m_db, m_date, this));
    m_tableView->horizontalHeader()->setStretchLastSection(true);

    auto* layout = new QVBoxLayout(this);
    layout->setContentsMargins(0,0,0,0);
    layout->addWidget(m_tableView);
    setLayout(layout);
}

void WeekTableWidget::setDate(const QDate& date) {
    m_date = date;
    m_tableView->setModel(new WeekAppointmentModel(m_db, m_date, this));
}

void WeekTableWidget::updateTheme() {
    // The base class handles theme propagation.
    // The global stylesheet in styles.qss.template
    // should be used to style QTableView.
}
