#pragma once
#include <QWidget>
#include <QLineEdit>
#include <QTableWidget>
#include "db/databasemanager.h"
#include "helper/modernbutton.h"
#include "client/clientdialog.h"
#include "client/editclientdialog.h"
#include "themes/themeablewidget.h"

class ClientPage : public ThemeableWidget {
    Q_OBJECT
public:
    explicit ClientPage(QWidget *parent = nullptr);

private slots:
    void onAddClientClicked();
    void onEditClient(const QModelIndex &index);
    void filterClients(const QString &text);

private:
    void setupUI();
    void setupConnections();
    void populateTable();
    void clearTable();

    ClientDB* db; // ✅ raw pointer since DatabaseManager owns it
    QLineEdit* searchBar;
    QTableWidget* clientsTable;
    ModernButton* addClientButton;
};
