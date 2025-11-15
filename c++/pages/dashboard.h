#ifndef DASHBOARD_H
#define DASHBOARD_H

#include "themes/themeablewidget.h"
#include <QStackedWidget>
#include <QVBoxLayout>
#include <QPushButton>
#include <QLabel>
#include <QSplitter>

#include "helper/modernbutton.h"
#include "home.h"
#include "statspage.h"
#include "quotepage.h"
#include "settingspage.h"
#include "documentspage.h"
#include "clientpage.h"
#include "appointmentpage.h"
#include "facebookpage.h"

#include "db/userdb.h"
#include "db/shopsettingsdb.h"
#include "db/clientdb.h"
#include "db/appointmentdb.h"
#include "db/documentstoragedb.h"
#include "helper/facebookclient.h"

class Dashboard : public ThemeableWidget {
    Q_OBJECT
public:
    enum PageIndex {
        Home = 0,
        Appointments,
        Clients,
        Documents,
        Quote,
        Stats,
        Settings,
        Facebook,
        PageCount
    };

    explicit Dashboard(const QString &username,
                       const QString &role,
                       ShopSettingDB* settingsDb,
                       UserDB* userDb,
                       ClientDB* clientDb,
                       AppointmentDB* appointmentDb,
                       DocumentStorageDB* documentsDb,
                       QWidget *parent = nullptr);

    void setFacebookPageVisibility(bool visible);

signals:
    void logoutRequested();

protected:
    void resizeEvent(QResizeEvent *event) override;

private slots:
    void onLogout();

private:
    void setupTopBar();
    void setupSidebar();
    void setupPages();
    void showPage(int index);
    void highlightSidebarButton(ModernButton* active);
    void refreshSidebar();
    void updateTheme() override;
    bool canAccessSettings() const;

    QWidget *m_topBar = nullptr;
    QWidget *m_sidebar = nullptr;
    QStackedWidget *m_stack = nullptr;
    QVBoxLayout *m_sideLayout = nullptr;

    ModernButton *homeBtn = nullptr;
    ModernButton *appointmentBtn = nullptr;
    ModernButton *clientsBtn = nullptr;
    ModernButton *documentsBtn = nullptr;
    ModernButton *quoteBtn = nullptr;
    ModernButton *statsBtn = nullptr;
    ModernButton *settingsBtn = nullptr;
    ModernButton *facebookBtn = nullptr;

    QPushButton *m_logoutBtn = nullptr;
    QLabel *m_userLabel = nullptr;
    QLabel *m_artwork = nullptr;
    QLabel *m_shopNameLabel = nullptr;
    QLabel *m_pricingLabel = nullptr;

    QString m_username;
    QString m_role;

    UserDB *m_userDb = nullptr;
    ShopSettingDB *m_settingsDb = nullptr;
    ClientDB *m_clientDb = nullptr;
    AppointmentDB *m_appointmentDb = nullptr;
    DocumentStorageDB *m_documentsDb = nullptr;
    FacebookClient* m_facebookClient = nullptr;

    QWidget* m_pages[PageCount] = { nullptr };
};

#endif // DASHBOARD_H