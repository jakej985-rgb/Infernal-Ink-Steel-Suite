#include "settingspage.h"
#include <QVBoxLayout>
#include <QTabBar>
#include <QFrame>
#include <QIcon>
#include <QPalette>
#include "helper/thememanager.h"
#include "themes/theme.h"

SettingsPage::SettingsPage(UserDB &udb,
                           ShopSettingDB &sdb,
                           FacebookClient* facebookClient,
                           const QString &currentUserRole,
                           const QString &currentUsername,
                           QWidget *parent)
    : ThemeableWidget(parent),
      userDB(udb),
      shopSettings(sdb),
      m_facebookClient(facebookClient),
      role(currentUserRole),
      username(currentUsername),
      tabWidget(nullptr),
      tabContainer(nullptr)
{
    tabContainer = new QFrame(this);
    tabContainer->setObjectName("settingsContainer");

    auto *containerLayout = new QVBoxLayout(tabContainer);
    containerLayout->setContentsMargins(12, 12, 12, 12);
    containerLayout->setSpacing(0);

    tabWidget = new QTabWidget(tabContainer);
    tabWidget->setObjectName("settingsTabWidget");
    tabWidget->setDocumentMode(true);
    QStringList roles = role.isEmpty() ? QStringList() : role.split("|", Qt::SkipEmptyParts);

    if (roles.contains("Admin")) {
        AdminTab *adminTab = new AdminTab(userDB, shopSettings, this);
        int idx = tabWidget->addTab(adminTab, tr("Admin"));
        tabWidget->setTabIcon(idx, QIcon(":/icons/clients.png"));
    }
    if (roles.contains("Admin") || roles.contains("Manager")) {
        ManagerTab *managerTab = new ManagerTab(userDB, this);
        int idx = tabWidget->addTab(managerTab, tr("Manager"));
        tabWidget->setTabIcon(idx, QIcon(":/icons/stats.png"));
    }
    UserTab *userTab = new UserTab(userDB, username, this);
    int userIdx = tabWidget->addTab(userTab, tr("User"));
    tabWidget->setTabIcon(userIdx, QIcon(":/icons/home.png"));
    // Linked Accounts tab
    LinkedAccountsTab *linkedAccountsTab = new LinkedAccountsTab(m_facebookClient, this);
    int linkedIdx = tabWidget->addTab(linkedAccountsTab, tr("Linked Accounts"));
    tabWidget->setTabIcon(linkedIdx, QIcon(":/icons/document.png"));
    connect(linkedAccountsTab, &LinkedAccountsTab::facebookAccountLinked, this, &SettingsPage::facebookAccountLinked);
    connect(linkedAccountsTab, &LinkedAccountsTab::facebookPageSelected, this, &SettingsPage::facebookPageSelected);

    // Theme tab (ThemeManager instance expected to exist)
    ThemesTab *themesTab = new ThemesTab(ThemeManager::instance(), username, this);
    int themeIdx = tabWidget->addTab(themesTab, tr("Theme"));
    tabWidget->setTabIcon(themeIdx, QIcon(":/icons/artwork.png"));

    containerLayout->addWidget(tabWidget);

    if (auto *bar = tabWidget->tabBar()) {
        bar->setObjectName("settingsTabBar");
    }

    auto *layout = new QVBoxLayout(this);
    layout->setContentsMargins(16, 16, 16, 16);
    layout->setSpacing(12);
    layout->addWidget(tabContainer);
    setLayout(layout);

    updateTheme();
}

void SettingsPage::updateTheme()
{
    ThemeableWidget::updateTheme();

    if (!tabWidget) {
        return;
    }

    Theme theme = ThemeManager::instance()->currentTheme();
    auto chooseColor = [](const QColor &first, const QColor &second, const QColor &fallback) {
        if (first.isValid()) {
            return first;
        }
        if (second.isValid()) {
            return second;
        }
        return fallback;
    };

    QColor surface = chooseColor(theme.backgroundColor, theme.background, palette().color(QPalette::Window));
    QColor text = chooseColor(theme.textColor, theme.text, palette().color(QPalette::WindowText));
    QColor accent = chooseColor(theme.accent, theme.primary, QColor("#3daee9"));
    QColor border = chooseColor(theme.borderColor, theme.border, accent.darker(150));
    QColor selectedText = chooseColor(theme.buttonTextColor, theme.textColor, QColor("#ffffff"));
    QColor hover = accent.isValid() ? QColor(accent).lighter(120) : QColor(border).lighter(130);
    QColor tabBase = surface.isValid() ? QColor(surface).lighter(108) : QColor("#2f2f2f");

    if (tabContainer) {
        tabContainer->setStyleSheet(QString(
            "#settingsContainer {"
            " background: %1;"
            " border-radius: 18px;"
            " border: 1px solid %2;"
            "}"
        ).arg(surface.name(), border.name()));
    }

    tabWidget->setStyleSheet(QString(
        "QTabWidget::pane {"
        " border: none;"
        " background: transparent;"
        "}"
        "QTabWidget::tab-bar {"
        " alignment: left;"
        " margin: 4px 12px;"
        "}"
    ));

    QPalette pal = tabWidget->palette();
    pal.setColor(QPalette::Base, surface);
    pal.setColor(QPalette::Window, surface);
    tabWidget->setPalette(pal);

    if (auto *bar = tabWidget->tabBar()) {
        bar->setStyleSheet(QString(
            "QTabBar#settingsTabBar {"
            " qproperty-drawBase: 0;"
            "}"
            "QTabBar#settingsTabBar::tab {"
            " color: %1;"
            " background: %2;"
            " padding: 6px 14px;"
            " margin-right: 8px;"
            " border-radius: 14px;"
            " border: 1px solid %3;"
            "}"
            "QTabBar#settingsTabBar::tab:selected {"
            " color: %4;"
            " background: %5;"
            " border-color: %5;"
            "}"
            "QTabBar#settingsTabBar::tab:hover {"
            " background: %6;"
            "}"
        ).arg(text.name(),
              tabBase.name(),
              border.name(),
              selectedText.name(),
              accent.name(),
              hover.name()));
    }
}

