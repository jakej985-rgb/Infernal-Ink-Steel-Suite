#pragma once
#include "themes/themeablewidget.h"
#include "db/userdb.h"
#include "db/shopsettingsdb.h"
#include "themes/themestab.h"
#include <QTabWidget>
#include "settingstabs/admintab.h"
#include "settingstabs/managertab.h"
#include "settingstabs/usertab.h"
#include "settingstabs/linkedaccountstab.h"
#include "helper/facebookclient.h"

class QFrame;

class SettingsPage : public ThemeableWidget
{
    Q_OBJECT
public:
    explicit SettingsPage(UserDB &udb, ShopSettingDB &sdb,
                          FacebookClient* facebookClient,
                          const QString &currentUserRole,
                          const QString &currentUsername,
                          QWidget *parent = nullptr);

    void updateTheme() override;

signals:
    void facebookAccountLinked(bool linked);
    void facebookPageSelected(const FacebookPageData& page);

private:
    UserDB &userDB;
    ShopSettingDB &shopSettings;
    FacebookClient* m_facebookClient;
    QString role;
    QString username;

    QTabWidget *tabWidget;
    QFrame *tabContainer;
};
