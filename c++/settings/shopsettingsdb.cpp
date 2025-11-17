#include "shopsettingsdb.h"
#include <QSqlQuery>
#include <QSqlError>
#include <QDebug>
#include <QStringList>

ShopSettingDB::ShopSettingDB(QSqlDatabase* database) : db(database) {}

bool ShopSettingDB::createTable() {
    QSqlQuery query(*db);
    bool ok = query.exec(
        "CREATE TABLE IF NOT EXISTS shopsettings ("
        "shopName TEXT, "
        "logoPath TEXT, "
        "accentColor TEXT, "
        "sidebarArtworkPath TEXT, "
        "loginHeadline TEXT, "
        "loginTagline TEXT, "
        "loginBackgroundPath TEXT, "
        "loginHeadlineFont TEXT, "
        "loginTaglineFont TEXT, "
        "loginTextColor TEXT, "
        "tattooPerHour REAL, "
        "piercingSingle REAL, "
        "piercingMulti REAL)"
    );
    if (!ok) qDebug() << "❌ ShopSettingDB createTable failed:" << query.lastError().text();
    else qDebug() << "✅ ShopSettingDB table ready.";

    if (ok) {
        const QStringList migrations = {
            "ALTER TABLE shopsettings ADD COLUMN loginHeadline TEXT",
            "ALTER TABLE shopsettings ADD COLUMN loginTagline TEXT",
            "ALTER TABLE shopsettings ADD COLUMN loginBackgroundPath TEXT",
            "ALTER TABLE shopsettings ADD COLUMN loginHeadlineFont TEXT",
            "ALTER TABLE shopsettings ADD COLUMN loginTaglineFont TEXT",
            "ALTER TABLE shopsettings ADD COLUMN loginTextColor TEXT"
        };
        for (const QString &sql : migrations) {
            QSqlQuery alter(*db);
            if (!alter.exec(sql)) {
                const QString err = alter.lastError().text();
                if (!err.contains("duplicate column", Qt::CaseInsensitive)) {
                    qDebug() << "ℹ️ Migration notice:" << err;
                }
            }
        }
    }
    return ok;
}

bool ShopSettingDB::saveSettings(const ShopSettings &settings) {
    if (!db->transaction()) {
        qDebug() << "❌ Failed to start transaction:" << db->lastError().text();
        return false;
    }

    QSqlQuery query(*db);
    if (!query.exec("DELETE FROM shopsettings")) {
        qDebug() << "❌ Failed to clear old settings:" << query.lastError().text();
        db->rollback();
        return false;
    }

    query.prepare("INSERT INTO shopsettings (shopName, logoPath, accentColor, sidebarArtworkPath, loginHeadline, loginTagline, loginBackgroundPath, loginHeadlineFont, loginTaglineFont, loginTextColor, tattooPerHour, piercingSingle, piercingMulti) "
                  "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)");
    query.addBindValue(settings.shopName);
    query.addBindValue(settings.logoPath);
    query.addBindValue(settings.accentColor);
    query.addBindValue(settings.sidebarArtworkPath);
    query.addBindValue(settings.loginHeadline);
    query.addBindValue(settings.loginTagline);
    query.addBindValue(settings.loginBackgroundPath);
    query.addBindValue(settings.loginHeadlineFontFamily);
    query.addBindValue(settings.loginTaglineFontFamily);
    query.addBindValue(settings.loginTextColor);
    query.addBindValue(settings.tattooPerHour);
    query.addBindValue(settings.piercingSingle);
    query.addBindValue(settings.piercingMulti);

    if(!query.exec()) {
        qDebug() << "❌ saveSettings failed:" << query.lastError().text();
        db->rollback();
        return false;
    }

    db->commit();
    qDebug() << "✅ Shop settings saved successfully.";
    return true;
}

ShopSettings ShopSettingDB::loadSettings() {
    ShopSettings s;
    QSqlQuery query(*db);
    if(!query.exec("SELECT * FROM shopsettings LIMIT 1")) {
        qDebug() << "❌ loadSettings failed:" << query.lastError().text();
        return s;
    }

    if(query.next()) {
        s.shopName = query.value("shopName").toString();
        s.logoPath = query.value("logoPath").toString();
        s.accentColor = query.value("accentColor").toString();
        s.sidebarArtworkPath = query.value("sidebarArtworkPath").toString();
        s.tattooPerHour = query.value("tattooPerHour").toDouble();
        s.piercingSingle = query.value("piercingSingle").toDouble();
        s.piercingMulti = query.value("piercingMulti").toDouble();
        s.loginHeadline = query.value("loginHeadline").toString();
        s.loginTagline = query.value("loginTagline").toString();
        s.loginBackgroundPath = query.value("loginBackgroundPath").toString();
        s.loginHeadlineFontFamily = query.value("loginHeadlineFont").toString();
        s.loginTaglineFontFamily = query.value("loginTaglineFont").toString();
        s.loginTextColor = query.value("loginTextColor").toString();
    } else {
        qDebug() << "⚠️ No shop settings found in database.";
    }

    return s;
}