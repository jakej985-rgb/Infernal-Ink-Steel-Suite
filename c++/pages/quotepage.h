#ifndef QUOTEPAGE_H
#define QUOTEPAGE_H

#include <QWidget>
#include <QVBoxLayout>
#include <QLabel>
#include <QPushButton>
#include <QTableWidget>
#include <QSqlDatabase>
#include <QSqlQuery>
#include <QSqlError>
#include <QMessageBox>

class QuotePage : public QWidget {
    Q_OBJECT
public:
    explicit QuotePage(QWidget *parent = nullptr);

private slots:
    void loadQuotes();

private:
    QLabel *tattooAvgLabel;
    QLabel *piercingAvgLabel;
    QTableWidget *quoteTable;

    double calculateAverage(const QString &serviceType);
};

#endif // QUOTEPAGE_H