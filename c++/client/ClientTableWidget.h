#pragma once
#include <QTableWidget>
#include "db/clientdb.h"
#include "themes/theme.h"

// Forward declaration to avoid heavy includes
class DatabaseManager;

class ClientTableWidget : public QTableWidget {
    Q_OBJECT
public:
    explicit ClientTableWidget(QWidget *parent = nullptr);

    void loadClients();
    void updateClient(const Client &client);
    void removeClient(int clientId);
    void applyTheme();

private:
    ClientDB *m_db = nullptr;  // use pointer for safety
};