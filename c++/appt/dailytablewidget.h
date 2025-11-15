#pragma once

#include "themes/themeablewidget.h"
#include "db/appointmentdb.h"
#include <QTableView>
#include <QDate>

class DailyTableWidget : public ThemeableWidget {
    Q_OBJECT

public:
    explicit DailyTableWidget(AppointmentDB* db, QWidget *parent = nullptr);

    void setDate(const QDate& date);

private:
    void updateTheme() override;
    QTableView* m_tableView;
    AppointmentDB* m_db;
    QDate m_date;
};
