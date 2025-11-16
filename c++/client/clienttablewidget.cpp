#include "ClientTableWidget.h"
#include "db/databasemanager.h"
#include "helper/thememanager.h"
#include <QTableWidgetItem>
#include <QHeaderView>

ClientTableWidget::ClientTableWidget(QWidget *parent)
    : QTableWidget(parent)
{
    // Use DatabaseManager singleton safely
    m_db = &DatabaseManager::instance()->clients();

    setColumnCount(5);
    setHorizontalHeaderLabels({ "First Name", "Middle Name", "Last Name", "Phone", "Email" });

    applyTheme();
    loadClients();

    horizontalHeader()->setStretchLastSection(true);
    setAlternatingRowColors(true);
    setSelectionBehavior(QAbstractItemView::SelectRows);
    setEditTriggers(QAbstractItemView::NoEditTriggers);
}

void ClientTableWidget::loadClients() {
    if (!m_db) return;

    auto clients = m_db->getAllClients();
    clearContents();
    setRowCount(clients.size());

    for (int row = 0; row < clients.size(); ++row) {
        const auto &c = clients[row];
        QStringList values = { c.firstName, c.middleName, c.lastName, c.phone, c.email };

        for (int col = 0; col < values.size(); ++col) {
            auto *item = new QTableWidgetItem(values[col]);
            item->setTextAlignment(Qt::AlignCenter);
            item->setFlags(item->flags() & ~Qt::ItemIsEditable);
            setItem(row, col, item);
        }
    }
}

void ClientTableWidget::updateClient(const Client &client) {
    if (m_db) {
        m_db->updateClient(client);
        loadClients();
    }
}

void ClientTableWidget::removeClient(int clientId) {
    if (m_db) {
        m_db->deleteClient(clientId);
        loadClients();
    }
}

void ClientTableWidget::applyTheme() {
    const Theme theme = ThemeManager::instance()->currentTheme();

    setStyleSheet(QString(
        "QTableWidget { background-color: %1; color: %2; font-family: '%3'; font-size: %4pt; "
        "alternate-background-color: %5; selection-background-color: %6; }")
        .arg(theme.background.name(),
             theme.text.name(),
             theme.font.family(),
             QString::number(theme.font.pointSize()),
             theme.background.lighter(105).name(),
             theme.primary.name()));

    setFont(theme.font);

}
