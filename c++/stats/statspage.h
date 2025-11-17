#ifndef STATSPAGE_H
#define STATSPAGE_H

#include <QWidget>
#include <QLabel>
#include <QComboBox>
#include <QVBoxLayout>
#include <QVector>
#include <QtCharts/QChartView>
#include <QtCharts/QChart>
#include <QtCharts/QBarSet>
#include <QtCharts/QBarSeries>
#include <QtCharts/QBarCategoryAxis>
#include <QtCharts/QValueAxis>
#include <QtCharts/QLineSeries>

class AppointmentDB;
class ShopSettingDB;

class StatsPage : public QWidget
{
    Q_OBJECT

public:
    explicit StatsPage(AppointmentDB *appointmentDb,
                       ShopSettingDB *settingsDb,
                       QWidget *parent = nullptr);

private slots:
    void updateCharts();

private:
    AppointmentDB *m_appointmentDb;
    ShopSettingDB *m_settingsDb;

    QVBoxLayout *mainLayout;

    QLabel *incomeLabel;
    QLabel *visitsLabel;
    QLabel *hoursLabel;
    QComboBox *yearSelector;

    QChartView *incomeChartView;
    QChartView *visitsChartView;
    QChartView *hoursChartView;

    void setupUI();
    void populateYearSelector();
    void loadData(int year,
                  QVector<double> &incomeData,
                  QVector<int> &visitsData,
                  QVector<double> &hoursData);
    bool shouldCountAppointment(const QString &status) const;
};

#endif // STATSPAGE_H