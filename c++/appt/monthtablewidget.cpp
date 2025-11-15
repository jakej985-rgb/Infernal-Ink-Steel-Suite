#include "monthtablewidget.h"
#include "helper/thememanager.h"
#include <QHeaderView>
#include <QStandardItemModel>
#include <QDate>

class MonthAppointmentModel : public QStandardItemModel {
public:
    MonthAppointmentModel(AppointmentDB* db, const QDate& date, QObject* parent = nullptr)
        : QStandardItemModel(parent), m_db(db), m_date(date) {

        QStringList daysOfWeek = {"Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"};
        setHorizontalHeaderLabels(daysOfWeek);

        int daysInMonth = m_date.daysInMonth();
        int firstDayOfWeek = m_date.addDays(-m_date.day() + 1).dayOfWeek();
        int numRows = (daysInMonth + firstDayOfWeek - 1) / 7 + 1;
        setRowCount(numRows);

        loadData();
    }

    void loadData() {
        int daysInMonth = m_date.daysInMonth();
        int firstDayOfWeek = m_date.addDays(-m_date.day() + 1).dayOfWeek();

        for (int day = 1; day <= daysInMonth; ++day) {
            QDate currentDate = m_date.addDays(-m_date.day() + day);
            int row = (day + firstDayOfWeek - 2) / 7;
            int col = (day + firstDayOfWeek - 2) % 7;

            auto appointments = m_db->getAppointmentsByDate(currentDate);
            QString cellText = QString::number(day);
            for (const auto& appt : appointments) {
                cellText += "\n" + appt.clientName;
            }
            QStandardItem* item = new QStandardItem(cellText);
            setItem(row, col, item);
        }
    }

private:
    AppointmentDB* m_db;
    QDate m_date;
};

#include <QVBoxLayout>

MonthTableWidget::MonthTableWidget(AppointmentDB* db, QWidget *parent)
    : ThemeableWidget(parent), m_db(db)
{
    m_tableView = new QTableView(this);
    m_date = QDate::currentDate();
    m_tableView->setModel(new MonthAppointmentModel(m_db, m_date, this));
    m_tableView->horizontalHeader()->setStretchLastSection(true);
    m_tableView->verticalHeader()->setSectionResizeMode(QHeaderView::Stretch);
    m_tableView->horizontalHeader()->setSectionResizeMode(QHeaderView::Stretch);

    auto* layout = new QVBoxLayout(this);
    layout->setContentsMargins(0,0,0,0);
    layout->addWidget(m_tableView);
    setLayout(layout);
}

void MonthTableWidget::setDate(const QDate& date) {
    m_date = date;
    m_tableView->setModel(new MonthAppointmentModel(m_db, m_date, this));
}

void MonthTableWidget::updateTheme() {
    // The base class handles theme propagation.
    // The global stylesheet in styles.qss.template
    // should be used to style QTableView.
}
