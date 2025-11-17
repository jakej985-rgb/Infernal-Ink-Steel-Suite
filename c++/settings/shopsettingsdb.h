#ifndef SHOPSETTINGDB_H
#define SHOPSETTINGDB_H

#include "domain/shopsettings.h"
#include <QSqlDatabase>

class ShopSettingDB {
public:
    explicit ShopSettingDB(QSqlDatabase* database);

    bool createTable();
    bool saveSettings(const ShopSettings &settings);
    ShopSettings loadSettings();

private:
    QSqlDatabase* db;
};

#endif // SHOPSETTINGDB_H
