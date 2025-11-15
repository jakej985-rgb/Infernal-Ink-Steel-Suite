#pragma once

#include "themes/themeablewidget.h"
#include <QTabWidget>
#include "helper/facebookclient.h"

class FacebookPage : public ThemeableWidget {
    Q_OBJECT

public:
    explicit FacebookPage(FacebookClient* client, QWidget *parent = nullptr);

public slots:
    void onPageSelected(const FacebookPageData& page);

private:
    FacebookClient* m_facebookClient;
    QTabWidget* m_tabWidget;
};
