// AppointmentLegendWidget.cpp
#include "appointmentlegendwidget.h"

AppointmentLegendWidget::AppointmentLegendWidget(QWidget* parent)
    : QWidget(parent)
{
    mainLayout = new QHBoxLayout(this);
    mainLayout->setSpacing(15);
    mainLayout->addStretch(1);
}

void AppointmentLegendWidget::updateLegend(const QMap<QString, QPair<QColor,int>>& entries) {
    // Clear existing widgets
    QLayoutItem* item;
    while ((item = mainLayout->takeAt(0)) != nullptr) {
        if (item->widget()) delete item->widget();
        delete item;
    }

    // Add standard labels first
    for (const QString& label : standardLabels) {
        if (entries.contains(label)) {
            auto pair = entries[label];
            mainLayout->addWidget(createColorBox(pair.first, label, pair.second));
        }
    }

    // Add remaining custom entries
    for (auto it = entries.begin(); it != entries.end(); ++it) {
        if (!standardLabels.contains(it.key())) {
            auto pair = it.value();
            mainLayout->addWidget(createColorBox(pair.first, it.key(), pair.second));
        }
    }

    mainLayout->addStretch(1);
}

QWidget* AppointmentLegendWidget::createColorBox(const QColor& color, const QString& text, int count) {
    auto* container = new QWidget(this);
    auto* hLayout = new QHBoxLayout(container);
    hLayout->setContentsMargins(0,0,0,0);
    hLayout->setSpacing(5);

    auto* box = new QLabel(container);
    box->setFixedSize(20, 20);
    box->setStyleSheet(QString("background-color: %1; border: 1px solid #000;").arg(color.name()));
    box->setToolTip(QString("%1 appointments").arg(count));

    auto* label = new QLabel(text, container);

    hLayout->addWidget(box);
    hLayout->addWidget(label);
    return container;
}