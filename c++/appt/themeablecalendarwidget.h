#pragma once

#include "themes/themeablewidget.h"
#include <QCalendarWidget>

class ThemeableCalendarWidget : public ThemeableWidget {
    Q_OBJECT

public:
    explicit ThemeableCalendarWidget(QWidget *parent = nullptr);

    QCalendarWidget* calendar() const { return m_calendar; }

private:
    void updateTheme() override;
    QCalendarWidget* m_calendar;
};
