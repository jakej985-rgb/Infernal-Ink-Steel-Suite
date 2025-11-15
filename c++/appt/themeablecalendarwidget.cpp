#include "themeablecalendarwidget.h"
#include "helper/thememanager.h"
#include <QVBoxLayout>

ThemeableCalendarWidget::ThemeableCalendarWidget(QWidget *parent)
    : ThemeableWidget(parent)
{
    m_calendar = new QCalendarWidget(this);
    auto* layout = new QVBoxLayout(this);
    layout->setContentsMargins(0,0,0,0);
    layout->addWidget(m_calendar);
    setLayout(layout);
}

void ThemeableCalendarWidget::updateTheme() {
    // The base class handles theme propagation.
    // The global stylesheet in styles.qss.template
    // should be used to style QCalendarWidget.
}
