#include "clientdb.h"

bool ClientDB::createTable() {
    if (!m_db) return false;
    QSqlQuery query(*m_db);
    QString sql = R"(
        CREATE TABLE IF NOT EXISTS clients (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            firstName TEXT,
            middleName TEXT,
            lastName TEXT,
            phone TEXT,
            email TEXT
        )
    )";
    bool ok = query.exec(sql);
    if(!ok) qDebug() << "❌ Failed to create clients table:" << query.lastError().text();
    else qDebug() << "✅ Clients table ready.";
    return ok;
}

bool ClientDB::addClient(const Client &client) {
    if (!m_db) return false;
    QSqlQuery query(*m_db);
    query.prepare("INSERT INTO clients (firstName, middleName, lastName, phone, email) VALUES (?, ?, ?, ?, ?)");
    query.addBindValue(client.firstName);
    query.addBindValue(client.middleName);
    query.addBindValue(client.lastName);
    query.addBindValue(client.phone);
    query.addBindValue(client.email);
    if (!query.exec()) {
        qDebug() << "❌ addClient failed:" << query.lastError().text();
        return false;
    }
    return true;
}

bool ClientDB::updateClient(const Client &client) {
    if (!m_db) return false;
    QSqlQuery query(*m_db);
    query.prepare("UPDATE clients SET firstName=?, middleName=?, lastName=?, phone=?, email=? WHERE id=?");
    query.addBindValue(client.firstName);
    query.addBindValue(client.middleName);
    query.addBindValue(client.lastName);
    query.addBindValue(client.phone);
    query.addBindValue(client.email);
    query.addBindValue(client.id);
    if (!query.exec()) {
        qDebug() << "❌ updateClient failed:" << query.lastError().text();
        return false;
    }

    if (query.numRowsAffected() == 0) {
        qWarning() << "⚠️ updateClient: no rows updated for id" << client.id;
        return false;
    }

    return true;
}

bool ClientDB::deleteClient(int clientId) {
    if (!m_db) return false;
    QSqlQuery query(*m_db);
    query.prepare("DELETE FROM clients WHERE id=?");
    query.addBindValue(clientId);
    if (!query.exec()) {
        qDebug() << "❌ deleteClient failed:" << query.lastError().text();
        return false;
    }

    if (query.numRowsAffected() == 0) {
        qWarning() << "⚠️ deleteClient: no client found for id" << clientId;
    }

    return true;
}

QList<Client> ClientDB::getAllClients() {
    QList<Client> list;
    if (!m_db) return list;

    QSqlQuery query(*m_db);
    query.exec("SELECT id, firstName, middleName, lastName, phone, email FROM clients");
    while (query.next()) {
        Client c;
        c.id = query.value(0).toInt();
        c.firstName = query.value(1).toString();
        c.middleName = query.value(2).toString();
        c.lastName = query.value(3).toString();
        c.phone = query.value(4).toString();
        c.email = query.value(5).toString();
        list.append(c);
    }
    return list;
}

std::optional<Client> ClientDB::getClientById(int id) {
    if (!m_db) return std::nullopt;

    QSqlQuery query(*m_db);
    query.prepare("SELECT id, firstName, middleName, lastName, phone, email FROM clients WHERE id=?");
    query.addBindValue(id);
    if (query.exec() && query.next()) {
        Client c;
        c.id = query.value(0).toInt();
        c.firstName = query.value(1).toString();
        c.middleName = query.value(2).toString();
        c.lastName = query.value(3).toString();
        c.phone = query.value(4).toString();
        c.email = query.value(5).toString();
        return c;
    }
    return std::nullopt;
}
