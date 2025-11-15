#ifndef HOME_H
#define HOME_H

#include <QWidget>
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QLabel>
#include <QPushButton>
#include <QTableWidget>
#include <QGroupBox>
#include <QFrame>
#include "themes/themeablewidget.h"

class Home : public ThemeableWidget
{
    Q_OBJECT
public:
    explicit Home(QWidget *parent = nullptr);

    void setWelcomeMessage(const QString &username);
    void updateStats(int income, int visits, int timeSpent);
    void setRecentActivity(const QList<QString> &activityList);

signals:
    void navigateToStatistics();
    void navigateToClients();
    void navigateToCalendar();

private:
    void updateTheme() override;
    QFrame* createStatCard(const QString &title, QLabel *&label, const QString &color);

    QVBoxLayout *m_mainLayout;
    QHBoxLayout *m_statsLayout;
    QHBoxLayout *m_navLayout;

    QLabel *m_welcomeLabel;
    QLabel *m_incomeLabel;
    QLabel *m_visitsLabel;
    QLabel *m_timeLabel;

    QFrame *m_incomeCard;
    QFrame *m_visitsCard;
    QFrame *m_timeCard;

    QTableWidget *m_recentActivityTable;
    QPushButton *m_statsButton;
    QPushButton *m_clientsButton;
    QPushButton *m_calendarButton;
};

#endif // HOME_H