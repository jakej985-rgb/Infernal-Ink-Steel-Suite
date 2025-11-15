#include "dashboard.h"
#include "config.h"
#include <QHBoxLayout>
#include <QSplitter>
#include <QPixmap>
#include <QResizeEvent>
#include <QMessageBox>
#include <QStringList>

// ------------------- Constructor -------------------
Dashboard::Dashboard(const QString &username,
                     const QString &role,
                     ShopSettingDB* settingsDb,
                     UserDB* userDb,
                     ClientDB* clientDb,
                     AppointmentDB* appointmentDb,
                     DocumentStorageDB* documentsDb,
                     QWidget *parent)
    : ThemeableWidget(parent),
      m_username(username),
      m_role(role),
      m_userDb(userDb),
      m_settingsDb(settingsDb),
      m_clientDb(clientDb),
      m_appointmentDb(appointmentDb),
      m_documentsDb(documentsDb)
{
    m_facebookClient = new FacebookClient(FACEBOOK_APP_ID, FACEBOOK_APP_SECRET, this);
    setMinimumSize(900, 600);

    auto *rootLayout = new QVBoxLayout(this);
    rootLayout->setContentsMargins(0,0,0,0);
    rootLayout->setSpacing(0);

    setupTopBar();
    rootLayout->addWidget(m_topBar);
    setupSidebar();
    setupPages();

    auto *splitter = new QSplitter(Qt::Horizontal);
    splitter->addWidget(m_sidebar);
    splitter->addWidget(m_stack);
    splitter->setStretchFactor(1, 1);
    rootLayout->addWidget(splitter);

    connect(m_logoutBtn, &QPushButton::clicked, this, &Dashboard::onLogout);

    connect(homeBtn, &ModernButton::clicked,        [this](){ showPage(Home); });
    connect(appointmentBtn, &ModernButton::clicked, [this](){ showPage(Appointments); });
    connect(clientsBtn, &ModernButton::clicked,     [this](){ showPage(Clients); });
    connect(documentsBtn, &ModernButton::clicked,   [this](){ showPage(Documents); });
    connect(quoteBtn, &ModernButton::clicked,       [this](){ showPage(Quote); });
    connect(statsBtn, &ModernButton::clicked,       [this](){ showPage(Stats); });
    connect(settingsBtn, &ModernButton::clicked,    [this](){ showPage(Settings); });
    connect(facebookBtn, &ModernButton::clicked,    [this](){ showPage(Facebook); });

    refreshSidebar();
    showPage(Home);
}

void Dashboard::setFacebookPageVisibility(bool visible)
{
    facebookBtn->setVisible(visible);
}

// ------------------- Top Bar -------------------
void Dashboard::setupTopBar()
{
    m_topBar = new QWidget(this);
    m_topBar->setObjectName("topBar");
    m_topBar->setMinimumHeight(42);

    auto *topLayout = new QHBoxLayout(m_topBar);
    topLayout->setContentsMargins(12, 0, 12, 0);

    m_userLabel = new QLabel(m_username, m_topBar);
    m_logoutBtn = new QPushButton("Logout", m_topBar);

    topLayout->addWidget(m_userLabel);
    topLayout->addStretch();
    topLayout->addWidget(m_logoutBtn);
}

// ------------------- Sidebar -------------------
void Dashboard::setupSidebar()
{
    m_sidebar = new QWidget(this);
    m_sidebar->setObjectName("sidebar");
    m_sidebar->setMinimumWidth(180);

    m_sideLayout = new QVBoxLayout(m_sidebar);
    m_sideLayout->setContentsMargins(8,8,8,8);

    homeBtn        = new ModernButton("Home", QIcon(":/icons/home.png"), m_sidebar);
    appointmentBtn = new ModernButton("Appointments", QIcon(":/icons/calendar.png"), m_sidebar);
    clientsBtn     = new ModernButton("Clients", QIcon(":/icons/clients.png"), m_sidebar);
    documentsBtn   = new ModernButton("Documents", QIcon(":/icons/document.png"), m_sidebar);
    quoteBtn       = new ModernButton("Quotes", QIcon(":/icons/login.png"), m_sidebar);
    statsBtn       = new ModernButton("Statistics", QIcon(":/icons/stats.png"), m_sidebar);
    settingsBtn    = new ModernButton("Settings", QIcon(":/icons/settings.png"), m_sidebar);
    facebookBtn    = new ModernButton("Facebook", QIcon(":/icons/facebook.png"), m_sidebar);
    facebookBtn->setVisible(false);

    for (auto btn : {homeBtn, appointmentBtn, clientsBtn, documentsBtn, quoteBtn, statsBtn})
        m_sideLayout->addWidget(btn);

    settingsBtn->setVisible(canAccessSettings());
    m_sideLayout->addWidget(facebookBtn);
    m_sideLayout->addWidget(settingsBtn);
    m_sideLayout->addStretch();

    m_artwork = new QLabel(m_sidebar);
    m_artwork->setAlignment(Qt::AlignCenter);
    m_shopNameLabel = new QLabel(m_sidebar);
    m_shopNameLabel->setAlignment(Qt::AlignCenter);
    m_pricingLabel = new QLabel(m_sidebar);
    m_pricingLabel->setAlignment(Qt::AlignCenter);

    m_sideLayout->addWidget(m_artwork);
    m_sideLayout->addWidget(m_shopNameLabel);
    m_sideLayout->addWidget(m_pricingLabel);
}

// ------------------- Pages -------------------
void Dashboard::setupPages()
{
    m_stack = new QStackedWidget(this);

    // Matches your /pages constructors
    m_pages[Home]         = new class Home(this);
    m_pages[Appointments] = new AppointmentsPage(m_appointmentDb, 0, true, this);
    m_pages[Clients]      = new ClientPage(this);
    m_pages[Documents]    = new DocumentsPage(m_documentsDb, 0, m_role, this);
    m_pages[Quote]        = new QuotePage(this);
    m_pages[Stats]        = new StatsPage(m_appointmentDb, m_settingsDb, this);
    m_pages[Settings]     = new SettingsPage(*m_userDb, *m_settingsDb, m_facebookClient, m_role, m_username, this);
    m_pages[Facebook]     = new FacebookPage(m_facebookClient, this);

    for (auto *page : m_pages)
        if (page) m_stack->addWidget(page);

    connect(static_cast<SettingsPage*>(m_pages[Settings]), &SettingsPage::facebookAccountLinked, this, &Dashboard::setFacebookPageVisibility);
    connect(static_cast<SettingsPage*>(m_pages[Settings]), &SettingsPage::facebookPageSelected, static_cast<FacebookPage*>(m_pages[Facebook]), &FacebookPage::onPageSelected);
}

// ------------------- Show Page -------------------
void Dashboard::showPage(int index)
{
    if (!m_stack || index < 0 || index >= PageCount) return;

    if (index == Settings && !canAccessSettings()) {
        QMessageBox::warning(this, "Access Denied", "You don't have permission to access Settings.");
        return;
    }

    m_stack->setCurrentIndex(index);

    ModernButton* active = nullptr;
    switch (index) {
        case Home: active = homeBtn; break;
        case Appointments: active = appointmentBtn; break;
        case Clients: active = clientsBtn; break;
        case Documents: active = documentsBtn; break;
        case Quote: active = quoteBtn; break;
        case Stats: active = statsBtn; break;
        case Settings: active = settingsBtn; break;
        case Facebook: active = facebookBtn; break;
    }

    highlightSidebarButton(active);
}

// ------------------- Highlight Sidebar -------------------
void Dashboard::highlightSidebarButton(ModernButton* active)
{
    for (auto btn : {homeBtn, appointmentBtn, clientsBtn, documentsBtn, quoteBtn, statsBtn, settingsBtn, facebookBtn}) {
        if (!btn) continue;
        if (btn == active)
            btn->setSelected(true);
        else
            btn->setSelected(false);
    }
}

bool Dashboard::canAccessSettings() const
{
    const QString normalizedSingle = m_role.trimmed();
    if (normalizedSingle.compare("admin", Qt::CaseInsensitive) == 0 ||
        normalizedSingle.compare("manager", Qt::CaseInsensitive) == 0) {
        return true;
    }

    const QStringList roles = m_role.split('|', Qt::SkipEmptyParts);
    for (const QString &role : roles) {
        const QString trimmed = role.trimmed();
        if (trimmed.compare("admin", Qt::CaseInsensitive) == 0 ||
            trimmed.compare("manager", Qt::CaseInsensitive) == 0) {
            return true;
        }
    }

    return false;
}

// ------------------- Refresh Sidebar -------------------
void Dashboard::refreshSidebar()
{
    if (!m_settingsDb) return;
    ShopSettings s = m_settingsDb->loadSettings();

    m_shopNameLabel->setText(s.shopName);

    QPixmap art(s.sidebarArtworkPath);
    if (!art.isNull())
        m_artwork->setPixmap(art.scaled(120,120,Qt::KeepAspectRatio,Qt::SmoothTransformation));
    else
        m_artwork->setText("[No Artwork]");

    m_pricingLabel->setText(
        QString("Tattoo/hr: $%1\nPiercing (Single): $%2\nPiercing (Multi): $%3")
            .arg(s.tattooPerHour)
            .arg(s.piercingSingle)
            .arg(s.piercingMulti)
    );
}

// ------------------- Apply Theme -------------------
void Dashboard::updateTheme()
{
    // The base class ThemeableWidget handles cascading theme updates.
    // Dashboard-specific theme updates can be added here if needed.
    // For now, we rely on the global stylesheet applied in main.cpp
    // and propagated by the ThemeableWidget::updateTheme implementation.
}

// ------------------- Logout -------------------
void Dashboard::onLogout()
{
    emit logoutRequested();
    close();
}

void Dashboard::resizeEvent(QResizeEvent *event)
{
    QWidget::resizeEvent(event);
}
