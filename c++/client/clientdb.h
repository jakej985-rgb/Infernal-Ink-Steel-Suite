#pragma once
#include "domain/client.h"
#include <QSqlDatabase>
#include <QSqlQuery>
#include <QDebug>
#include <QList>
#include <optional>

class ClientDB {
public:
    explicit ClientDB(QSqlDatabase* db) : m_db(db) {}

    bool createTable();
    bool addClient(const Client &client);
    bool updateClient(const Client &client);
    bool deleteClient(int clientId);

    QList<Client> getAllClients();
    std::optional<Client> getClientById(int id);

private:
    QSqlDatabase* m_db;
};
