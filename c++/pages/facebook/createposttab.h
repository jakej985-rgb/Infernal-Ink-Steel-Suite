#pragma once

#include "themes/themeablewidget.h"
#include <QTextEdit>
#include <QPushButton>
#include <QLineEdit>
#include "helper/facebookclient.h"

class CreatePostTab : public ThemeableWidget {
    Q_OBJECT

public:
    explicit CreatePostTab(FacebookClient* client, QWidget *parent = nullptr);

    void setCurrentPage(const FacebookPageData& page);

private slots:
    void onPostButtonClicked();
    void onPostCreated(bool success);

private:
    FacebookClient* m_facebookClient;
    FacebookPageData m_currentPage;
    QTextEdit* m_postTextEdit;
    QLineEdit* m_imagePathLineEdit;
    QPushButton* m_attachImageButton;
    QPushButton* m_postButton;
};
