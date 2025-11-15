#include "quotepage.h"
#include <QHeaderView>

QuotePage::QuotePage(QWidget *parent) : QWidget(parent) {
    auto *layout = new QVBoxLayout(this);

    tattooAvgLabel = new QLabel("Average Tattoo Price/hr: Loading...", this);
    piercingAvgLabel = new QLabel("Average Piercing Price/hr: Loading...", this);

    quoteTable = new QTableWidget(this);
    quoteTable->setColumnCount(4);
    quoteTable->setHorizontalHeaderLabels({"Service", "Price Charged", "Duration (min)", "Price/hr"});
    quoteTable->horizontalHeader()->setStretchLastSection(true);
    quoteTable->setEditTriggers(QAbstractItemView::NoEditTriggers);
    quoteTable->setSelectionBehavior(QAbstractItemView::SelectRows);
    quoteTable->setAlternatingRowColors(true);

    auto *refreshBtn = new QPushButton("Refresh Quotes", this);
    connect(refreshBtn, &QPushButton::clicked, this, &QuotePage::loadQuotes);

    layout->addWidget(tattooAvgLabel);
    layout->addWidget(piercingAvgLabel);
    layout->addWidget(quoteTable);
    layout->addWidget(refreshBtn);

    setLayout(layout);
    loadQuotes();
}

void QuotePage::loadQuotes() {
    QSqlQuery query;
    if (!query.exec("SELECT serviceType, priceCharged, durationMinutes FROM appointments WHERE status='complete'")) {
        QMessageBox::warning(this, "DB Error", query.lastError().text());
        return;
    }

    quoteTable->setRowCount(0);

    while (query.next()) {
        QString service = query.value(0).toString();
        double price = query.value(1).toDouble();
        int duration = query.value(2).toInt();

        double pricePerHr = 0.0;
        if (duration > 0)
            pricePerHr = price / (duration / 60.0);

        int row = quoteTable->rowCount();
        quoteTable->insertRow(row);
        auto *serviceItem = new QTableWidgetItem(service);
        auto *priceItem = new QTableWidgetItem(QString("$%1").arg(QString::number(price, 'f', 2)));
        auto *durationItem = new QTableWidgetItem(QString::number(duration));
        auto *pricePerHourItem = new QTableWidgetItem(QString("$%1").arg(QString::number(pricePerHr, 'f', 2)));

        priceItem->setTextAlignment(Qt::AlignRight | Qt::AlignVCenter);
        durationItem->setTextAlignment(Qt::AlignRight | Qt::AlignVCenter);
        pricePerHourItem->setTextAlignment(Qt::AlignRight | Qt::AlignVCenter);

        quoteTable->setItem(row, 0, serviceItem);
        quoteTable->setItem(row, 1, priceItem);
        quoteTable->setItem(row, 2, durationItem);
        quoteTable->setItem(row, 3, pricePerHourItem);
    }

    tattooAvgLabel->setText(QString("Average Tattoo Price/hr: $%1")
                                .arg(QString::number(calculateAverage("tattoo"), 'f', 2)));
    piercingAvgLabel->setText(QString("Average Piercing Price/hr: $%1")
                                  .arg(QString::number(calculateAverage("piercing"), 'f', 2)));
}

double QuotePage::calculateAverage(const QString &serviceType) {
    QSqlQuery query;
    query.prepare("SELECT priceCharged, durationMinutes FROM appointments WHERE serviceType=? AND status='complete'");
    query.addBindValue(serviceType);

    if (!query.exec()) {
        return 0.0;
    }

    double total = 0.0;
    int count = 0;
    while (query.next()) {
        double price = query.value(0).toDouble();
        int duration = query.value(1).toInt();
        if (duration > 0) {
            total += price / (duration / 60.0);
            count++;
        }
    }

    return (count > 0) ? (total / count) : 0.0;
}