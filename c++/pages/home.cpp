#include "home.h"
#include <QFont>
#include <QPalette>
#include <QHeaderView>
#include "helper/thememanager.h"

Home::Home(QWidget *parent) : ThemeableWidget(parent)
{
    m_mainLayout = new QVBoxLayout(this);
    m_mainLayout->setSpacing(20);
    m_mainLayout->setContentsMargins(20, 20, 20, 20);

    // Welcome Label
    m_welcomeLabel = new QLabel("Welcome!", this);
    m_welcomeLabel->setAlignment(Qt::AlignCenter);
    m_mainLayout->addWidget(m_welcomeLabel);

    // Stats Layout
    m_statsLayout = new QHBoxLayout();
    m_statsLayout->setSpacing(20);

    m_incomeCard = createStatCard("Income", m_incomeLabel, "");
    m_visitsCard = createStatCard("Visits", m_visitsLabel, "");
    m_timeCard = createStatCard("Time Spent", m_timeLabel, "");

    m_statsLayout->addWidget(m_incomeCard);
    m_statsLayout->addWidget(m_visitsCard);
    m_statsLayout->addWidget(m_timeCard);

    m_mainLayout->addLayout(m_statsLayout);

    // Recent Activity Table
    m_recentActivityTable = new QTableWidget(this);
    m_recentActivityTable->setColumnCount(2);
    m_recentActivityTable->setHorizontalHeaderLabels({"Client", "Action"});
    m_recentActivityTable->horizontalHeader()->setStretchLastSection(true);
    m_recentActivityTable->setEditTriggers(QAbstractItemView::NoEditTriggers);
    m_recentActivityTable->setSelectionBehavior(QAbstractItemView::SelectRows);
    m_recentActivityTable->setAlternatingRowColors(true);

    QGroupBox *recentGroup = new QGroupBox("Recent Activity", this);
    QVBoxLayout *recentGroupLayout = new QVBoxLayout();
    recentGroupLayout->addWidget(m_recentActivityTable);
    recentGroup->setLayout(recentGroupLayout);

    m_mainLayout->addWidget(recentGroup);

    // Navigation Buttons
    m_navLayout = new QHBoxLayout();
    m_navLayout->setSpacing(20);

    m_statsButton = new QPushButton("Statistics", this);
    m_clientsButton = new QPushButton("Clients", this);
    m_calendarButton = new QPushButton("Calendar", this);

    m_navLayout->addWidget(m_statsButton);
    m_navLayout->addWidget(m_clientsButton);
    m_navLayout->addWidget(m_calendarButton);

    m_mainLayout->addLayout(m_navLayout);

    // Connect buttons
    connect(m_statsButton, &QPushButton::clicked, this, &Home::navigateToStatistics);
    connect(m_clientsButton, &QPushButton::clicked, this, &Home::navigateToClients);
    connect(m_calendarButton, &QPushButton::clicked, this, &Home::navigateToCalendar);

    setLayout(m_mainLayout);
}

void Home::updateTheme()
{
    Theme t = ThemeManager::instance()->currentTheme();
    m_welcomeLabel->setStyleSheet(QString("font-size: 28px; font-weight: bold; color:%1;").arg(t.text.name()));
    m_incomeCard->setStyleSheet(QString("background-color: %1; border-radius: 10px; padding: 20px; color: white;").arg(t.primary.name()));
    m_visitsCard->setStyleSheet(QString("background-color: %1; border-radius: 10px; padding: 20px; color: white;").arg(t.secondary.name()));
    m_timeCard->setStyleSheet(QString("background-color: %1; border-radius: 10px; padding: 20px; color: white;").arg(t.accent.name()));
    m_recentActivityTable->setStyleSheet(
        QString("QTableWidget { background-color: %1; alternate-background-color:%2; color:%3; }")
        .arg(t.background.darker(115).name())
        .arg(t.background.darker(130).name())
        .arg(t.text.name())
    );
    QGroupBox *recentGroup = findChild<QGroupBox *>();
    if(recentGroup)
        recentGroup->setStyleSheet(QString("QGroupBox { color:%1; font-weight:bold; }").arg(t.text.name()));
    styleButton(m_statsButton);
    styleButton(m_clientsButton);
    styleButton(m_calendarButton);
}

QFrame* Home::createStatCard(const QString &title, QLabel *&label, const QString &)
{
    QFrame *card = new QFrame(this);
    card->setFrameShape(QFrame::StyledPanel);

    QVBoxLayout *layout = new QVBoxLayout(card);

    QLabel *titleLabel = new QLabel(title, card);
    titleLabel->setStyleSheet("font-size: 16px; font-weight: bold;");
    titleLabel->setAlignment(Qt::AlignCenter);

    label = new QLabel("0", card);
    QFont f = label->font();
    f.setPointSize(24);
    f.setBold(true);
    label->setFont(f);
    label->setAlignment(Qt::AlignCenter);

    layout->addWidget(titleLabel);
    layout->addWidget(label);

    return card;
}

void Home::setWelcomeMessage(const QString &username)
{
    m_welcomeLabel->setText(QString("Welcome, %1!").arg(username));
}

void Home::updateStats(int income, int visits, int timeSpent)
{
    m_incomeLabel->setText(QString("$%1").arg(income));
    m_visitsLabel->setText(QString::number(visits));
    m_timeLabel->setText(QString("%1 min").arg(timeSpent));
}

void Home::setRecentActivity(const QList<QString> &activityList)
{
    m_recentActivityTable->setRowCount(activityList.size());
    for (int i = 0; i < activityList.size(); ++i) {
        QStringList parts = activityList[i].split("|"); // format: "Client|Action"
        m_recentActivityTable->setItem(i, 0, new QTableWidgetItem(parts.value(0)));
        m_recentActivityTable->setItem(i, 1, new QTableWidgetItem(parts.value(1)));
    }

    if(m_recentActivityTable->rowCount() > 0)
        m_recentActivityTable->selectRow(0);
}