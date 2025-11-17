#include "statspage.h"
#include "db/appointmentdb.h"
#include "db/shopsettingsdb.h"
#include "domain/appointment.h"

#include <QDate>
#include <QSet>
#include <QTime>
#include <algorithm>
#include <numeric>
#include <QtCharts/QCategoryAxis>
#include <QtCharts/QLineSeries>

namespace {
    constexpr int kMonthsInYear = 12;

    inline double minutesToHours(int minutes)
    {
        return static_cast<double>(minutes) / 60.0;
    }
}

StatsPage::StatsPage(AppointmentDB *appointmentDb,
                     ShopSettingDB *settingsDb,
                     QWidget *parent)
    : QWidget(parent),
      m_appointmentDb(appointmentDb),
      m_settingsDb(settingsDb)
{
    setupUI();
    populateYearSelector();
    updateCharts();
}

void StatsPage::setupUI()
{
    mainLayout = new QVBoxLayout(this);
    mainLayout->setSpacing(15);
    mainLayout->setContentsMargins(10, 10, 10, 10);

    // Year selector
    yearSelector = new QComboBox(this);
    connect(yearSelector, &QComboBox::currentTextChanged, this, &StatsPage::updateCharts);
    mainLayout->addWidget(yearSelector);

    // Summary labels
    incomeLabel = new QLabel("Total Income: $0", this);
    visitsLabel = new QLabel("Total Visits: 0", this);
    hoursLabel  = new QLabel("Total Tattoo Hours: 0", this);

    mainLayout->addWidget(incomeLabel);
    mainLayout->addWidget(visitsLabel);
    mainLayout->addWidget(hoursLabel);

    // Chart placeholders
    incomeChartView = new QChartView(this);
    visitsChartView = new QChartView(this);
    hoursChartView  = new QChartView(this);

    mainLayout->addWidget(incomeChartView);
    mainLayout->addWidget(visitsChartView);
    mainLayout->addWidget(hoursChartView);
}

void StatsPage::populateYearSelector()
{
    yearSelector->blockSignals(true);
    yearSelector->clear();

    QSet<int> years;
    if (m_appointmentDb) {
        const QVector<Appointment> allAppointments = m_appointmentDb->getAllAppointments();
        for (const Appointment &appt : allAppointments) {
            if (!appt.dateTime.isValid())
                continue;
            years.insert(appt.dateTime.date().year());
        }
    }

    if (years.isEmpty())
        years.insert(QDate::currentDate().year());

    QList<int> sortedYears = years.values();
    std::sort(sortedYears.begin(), sortedYears.end(), std::greater<int>());

    for (int year : sortedYears)
        yearSelector->addItem(QString::number(year));

    yearSelector->blockSignals(false);
    if (yearSelector->count() > 0)
        yearSelector->setCurrentIndex(0);
}

void StatsPage::loadData(int year,
                         QVector<double> &incomeData,
                         QVector<int> &visitsData,
                         QVector<double> &hoursData)
{
    incomeData = QVector<double>(kMonthsInYear, 0.0);
    visitsData = QVector<int>(kMonthsInYear, 0);
    hoursData  = QVector<double>(kMonthsInYear, 0.0);

    if (!m_appointmentDb)
        return;

    const QDate startDate(year, 1, 1);
    const QDate endDate(year, 12, 31);
    const QDateTime startDateTime(startDate, QTime(0, 0, 0));
    const QDateTime endDateTime(endDate, QTime(23, 59, 59, 999));

    const QVector<Appointment> appointments = m_appointmentDb->getAppointmentsBetween(startDateTime, endDateTime);

    double hourlyRate = 0.0;
    if (m_settingsDb) {
        const ShopSettings settings = m_settingsDb->loadSettings();
        hourlyRate = settings.tattooPerHour;
    }

    for (const Appointment &appt : appointments) {
        if (!appt.dateTime.isValid())
            continue;

        const int monthIndex = appt.dateTime.date().month() - 1;
        if (monthIndex < 0 || monthIndex >= kMonthsInYear)
            continue;

        if (!shouldCountAppointment(appt.status))
            continue;

        visitsData[monthIndex] += 1;
        hoursData[monthIndex] += minutesToHours(appt.durationMinutes);

        double income = appt.priceCharged;
        if (qFuzzyIsNull(income) && hourlyRate > 0.0)
            income = minutesToHours(appt.durationMinutes) * hourlyRate;

        incomeData[monthIndex] += income;
    }
}

void StatsPage::updateCharts()
{
    if (yearSelector->count() == 0)
        return;

    bool ok = false;
    int year = yearSelector->currentText().toInt(&ok);
    if (!ok)
        year = QDate::currentDate().year();

    QVector<double> incomeData;
    QVector<int> visitsData;
    QVector<double> hoursData;

    loadData(year, incomeData, visitsData, hoursData);

    // --- Summary ---
    double totalIncome = std::accumulate(incomeData.begin(), incomeData.end(), 0.0);
    int totalVisits = std::accumulate(visitsData.begin(), visitsData.end(), 0);
    double totalHours = std::accumulate(hoursData.begin(), hoursData.end(), 0.0);

    incomeLabel->setText(QString("Total Income: $%1").arg(totalIncome, 0, 'f', 2));
    visitsLabel->setText(QString("Total Visits: %1").arg(totalVisits));
    hoursLabel->setText(QString("Total Tattoo Hours: %1").arg(totalHours, 0, 'f', 1));

    QStringList months = {"Jan","Feb","Mar","Apr","May","Jun",
                          "Jul","Aug","Sep","Oct","Nov","Dec"};

    // --- Income Chart ---
    {
        QBarSet *set = new QBarSet("Income");
        for (double v : incomeData) *set << v;

        QBarSeries *series = new QBarSeries();
        series->append(set);

        QChart *chart = new QChart();
        chart->addSeries(series);
        chart->setTitle(QString("Monthly Income - %1").arg(year));

        QBarCategoryAxis *axisX = new QBarCategoryAxis();
        axisX->append(months);
        chart->addAxis(axisX, Qt::AlignBottom);
        series->attachAxis(axisX);

        QValueAxis *axisY = new QValueAxis();
        axisY->setTitleText("Income ($)");
        axisY->setMin(0);
        chart->addAxis(axisY, Qt::AlignLeft);
        series->attachAxis(axisY);

        incomeChartView->setChart(chart);
    }

    // --- Visits Chart ---
    {
        QLineSeries *series = new QLineSeries();
        for (int i = 0; i < visitsData.size(); ++i)
            series->append(i, visitsData[i]);

        QChart *chart = new QChart();
        chart->addSeries(series);
        chart->setTitle(QString("Monthly Visits - %1").arg(year));

        QCategoryAxis *axisX = new QCategoryAxis();
        for (int i = 0; i < months.size(); ++i)
            axisX->append(months[i], i);
        chart->addAxis(axisX, Qt::AlignBottom);
        series->attachAxis(axisX);

        QValueAxis *axisY = new QValueAxis();
        axisY->setTitleText("Visits");
        axisY->setMin(0);
        chart->addAxis(axisY, Qt::AlignLeft);
        series->attachAxis(axisY);

        visitsChartView->setChart(chart);
    }

    // --- Tattoo Hours Chart ---
    {
        QLineSeries *series = new QLineSeries();
        for (int i = 0; i < hoursData.size(); ++i)
            series->append(i, hoursData[i]);

        QChart *chart = new QChart();
        chart->addSeries(series);
        chart->setTitle(QString("Monthly Tattoo Hours - %1").arg(year));

        QCategoryAxis *axisX = new QCategoryAxis();
        for (int i = 0; i < months.size(); ++i)
            axisX->append(months[i], i);
        chart->addAxis(axisX, Qt::AlignBottom);
        series->attachAxis(axisX);

        QValueAxis *axisY = new QValueAxis();
        axisY->setTitleText("Hours");
        axisY->setMin(0);
        chart->addAxis(axisY, Qt::AlignLeft);
        series->attachAxis(axisY);

        hoursChartView->setChart(chart);
    }
}

bool StatsPage::shouldCountAppointment(const QString &status) const
{
    const QString normalized = status.trimmed().toLower();
    return !(normalized == "canceled" ||
             normalized == "cancelled" ||
             normalized == "no show" ||
             normalized == "noshow");
}
