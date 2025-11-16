#ifndef SHOPSETTINGS_H
#define SHOPSETTINGS_H

#include <QString>
#include <QDateTime>
#include <optional>
#include <QSqlDatabase>
#include <QSqlQuery>
#include <QSqlError>
#include <QDebug>

struct ShopSettings {
    int id = -1;
    QString shopName;
    QString logoPath;
    QString accentColor;
    QString sidebarArtworkPath;
    QString loginHeadline;
    QString loginTagline;
    QString loginBackgroundPath;
    QString loginHeadlineFontFamily;
    QString loginTaglineFontFamily;
    QString loginTextColor;
    double tattooPerHour = 0.0;
    double piercingSingle = 0.0;
    double piercingMulti = 0.0;
    QDateTime createdAt = QDateTime::currentDateTime();
    QDateTime updatedAt = QDateTime::currentDateTime();

    static std::optional<ShopSettings> getById(const QSqlDatabase &db, int settingsId) {
        QSqlQuery query(db);
        query.prepare("SELECT * FROM shop_settings WHERE id=:id");
        query.bindValue(":id", settingsId);

        if (query.exec() && query.next()) {
            ShopSettings s;
            s.id = query.value("id").toInt();
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
            s.createdAt = QDateTime::fromString(query.value("createdAt").toString(), Qt::ISODate);
            s.updatedAt = QDateTime::fromString(query.value("updatedAt").toString(), Qt::ISODate);
            return s;
        }
        return std::nullopt;
    }

    bool insert(const QSqlDatabase &db) {
        QSqlQuery query(db);
        query.prepare("INSERT INTO shop_settings (shopName, logoPath, accentColor, sidebarArtworkPath, "
                      "loginHeadline, loginTagline, loginBackgroundPath, loginHeadlineFont, loginTaglineFont, loginTextColor, "
                      "tattooPerHour, piercingSingle, piercingMulti, createdAt, updatedAt) "
                      "VALUES (:name, :logo, :accent, :sidebar, :headline, :tagline, :background, :headlineFont, :taglineFont, :textColor, :tattoo, :single, :multi, :created, :updated)");
        query.bindValue(":name", shopName);
        query.bindValue(":logo", logoPath);
        query.bindValue(":accent", accentColor);
        query.bindValue(":sidebar", sidebarArtworkPath);
        query.bindValue(":headline", loginHeadline);
        query.bindValue(":tagline", loginTagline);
        query.bindValue(":background", loginBackgroundPath);
        query.bindValue(":headlineFont", loginHeadlineFontFamily);
        query.bindValue(":taglineFont", loginTaglineFontFamily);
        query.bindValue(":textColor", loginTextColor);
        query.bindValue(":tattoo", tattooPerHour);
        query.bindValue(":single", piercingSingle);
        query.bindValue(":multi", piercingMulti);
        query.bindValue(":created", createdAt.toString(Qt::ISODate));
        query.bindValue(":updated", updatedAt.toString(Qt::ISODate));

        if (query.exec()) {
            id = query.lastInsertId().toInt();
            return true;
        } else {
            qDebug() << "ShopSettings insert failed:" << query.lastError().text();
            return false;
        }
    }

    bool update(const QSqlDatabase &db) {
        if (id == -1) return false;

        QSqlQuery query(db);
        query.prepare("UPDATE shop_settings SET shopName=:name, logoPath=:logo, accentColor=:accent, "
                      "sidebarArtworkPath=:sidebar, loginHeadline=:headline, loginTagline=:tagline, loginBackgroundPath=:background, "
                      "loginHeadlineFont=:headlineFont, loginTaglineFont=:taglineFont, loginTextColor=:textColor, "
                      "tattooPerHour=:tattoo, piercingSingle=:single, piercingMulti=:multi, createdAt=:created, updatedAt=:updated WHERE id=:id");
        query.bindValue(":name", shopName);
        query.bindValue(":logo", logoPath);
        query.bindValue(":accent", accentColor);
        query.bindValue(":sidebar", sidebarArtworkPath);
        query.bindValue(":headline", loginHeadline);
        query.bindValue(":tagline", loginTagline);
        query.bindValue(":background", loginBackgroundPath);
        query.bindValue(":headlineFont", loginHeadlineFontFamily);
        query.bindValue(":taglineFont", loginTaglineFontFamily);
        query.bindValue(":textColor", loginTextColor);
        query.bindValue(":tattoo", tattooPerHour);
        query.bindValue(":single", piercingSingle);
        query.bindValue(":multi", piercingMulti);
        query.bindValue(":created", createdAt.toString(Qt::ISODate));
        query.bindValue(":updated", updatedAt.toString(Qt::ISODate));
        query.bindValue(":id", id);

        if (!query.exec()) {
            qDebug() << "ShopSettings update failed:" << query.lastError().text();
            return false;
        }
        return true;
    }
};

#endif // SHOPSETTINGS_H
