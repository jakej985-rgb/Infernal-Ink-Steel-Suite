#include "clientpage.h"
#include <QVBoxLayout>
#include <QHeaderView>
#include <QDebug>
#include "domain/client.h"
#include "qdialog.h"

ClientPage::ClientPage(QWidget *parent)
    : ThemeableWidget(parent)
{
    // ✅ Use singleton DatabaseManager
    db = &DatabaseManager::instance()->clients();

    setupUI();
    setupConnections();

    if (db && !db->createTable())
        qDebug() << "Failed to create clients table!";

    populateTable();
}

void ClientPage::setupUI() {
    auto* layout = new QVBoxLayout(this);

    searchBar = new QLineEdit(this);
    searchBar->setPlaceholderText("Search clients...");
    layout->addWidget(searchBar);

    clientsTable = new QTableWidget(0, 6, this);
    clientsTable->setHorizontalHeaderLabels({"ID", "First", "Middle", "Last", "Phone", "Email"});
    clientsTable->horizontalHeader()->setStretchLastSection(true);
    clientsTable->setSelectionBehavior(QAbstractItemView::SelectRows);
    clientsTable->setEditTriggers(QAbstractItemView::NoEditTriggers);
    clientsTable->setSelectionMode(QAbstractItemView::SingleSelection);
    layout->addWidget(clientsTable);

    addClientButton = new ModernButton("Add Client", QIcon(), this);
    layout->addWidget(addClientButton);

    setLayout(layout);
}

void ClientPage::setupConnections() {
    connect(searchBar, &QLineEdit::textChanged, this, &ClientPage::filterClients);
    connect(addClientButton, &QPushButton::clicked, this, &ClientPage::onAddClientClicked);
    connect(clientsTable, &QTableWidget::doubleClicked, this, &ClientPage::onEditClient);
}

void ClientPage::populateTable() {
    if (!db) return;
    clearTable();

    QList<Client> clients = db->getAllClients();
    for (const Client &c : clients) {
        int row = clientsTable->rowCount();
        clientsTable->insertRow(row);
        clientsTable->setItem(row, 0, new QTableWidgetItem(QString::number(c.id)));
        clientsTable->setItem(row, 1, new QTableWidgetItem(c.firstName));
        clientsTable->setItem(row, 2, new QTableWidgetItem(c.middleName));
        clientsTable->setItem(row, 3, new QTableWidgetItem(c.lastName));
        clientsTable->setItem(row, 4, new QTableWidgetItem(c.phone));
        clientsTable->setItem(row, 5, new QTableWidgetItem(c.email));
    }
}

void ClientPage::clearTable() {
    clientsTable->setRowCount(0);
}

void ClientPage::onAddClientClicked() {
    if (!db) return;

    ClientDialog dialog(this);
    if (dialog.exec() != QDialog::Accepted) return;

    Client c = dialog.getClient();
    if (!db->addClient(c)) {
        qDebug() << "Client add failed.";
        return;
    }

    populateTable();
}

void ClientPage::onEditClient(const QModelIndex &index) {
    if (!db || !index.isValid()) return;

    int row = index.row();
    int id = clientsTable->item(row, 0)->text().toInt();

    // ✅ We can call getClientById directly from ClientDB
    auto clientOpt = db->getClientById(id);
    if (!clientOpt.has_value()) return;

    Client c = clientOpt.value();
    EditClientDialog dialog(&DatabaseManager::instance()->getDatabase(), this);
    dialog.setClient(c);
    if (dialog.exec() == QDialog::Accepted) {
        Client updated = dialog.getClient();
        if (!db->updateClient(updated))
            qDebug() << "Failed to update client.";
        populateTable();
        clientsTable->selectRow(row);
    }
}

void ClientPage::filterClients(const QString &text) {
    for (int row = 0; row < clientsTable->rowCount(); ++row) {
        bool match = false;
        for (int col = 1; col <= 5; ++col) {
            QTableWidgetItem *item = clientsTable->item(row, col);
            if (item && item->text().contains(text, Qt::CaseInsensitive)) {
                match = true;
                break;
            }
        }
        clientsTable->setRowHidden(row, !match);
    }
}
