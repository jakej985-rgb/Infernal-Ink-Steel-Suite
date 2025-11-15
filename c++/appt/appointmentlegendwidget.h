// AppointmentLegendWidget.h
#pragma once

#include <QWidget>
#include <QLabel>
#include <QHBoxLayout>
#include <QMap>
#include <QColor>

class AppointmentLegendWidget : public QWidget {
    Q_OBJECT
public:
    explicit AppointmentLegendWidget(QWidget* parent = nullptr);

    // key = label, value = pair(color, count)
    void updateLegend(const QMap<QString, QPair<QColor,int>>& entries);

private:
    QWidget* createColorBox(const QColor& color, const QString& text, int count);
    QHBoxLayout* mainLayout;
    QStringList standardLabels = {"Past", "Today", "Future"};
};