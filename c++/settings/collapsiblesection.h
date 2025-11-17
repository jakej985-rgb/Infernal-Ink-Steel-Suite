#pragma once

#include "themes/themeablewidget.h"

#include <QIcon>

class QLabel;
class QToolButton;
class QVBoxLayout;

class CollapsibleSection : public ThemeableWidget
{
    Q_OBJECT
public:
    explicit CollapsibleSection(const QString &title,
                                const QIcon &icon,
                                QWidget *parent = nullptr);

    QVBoxLayout *contentLayout() const;
    void setCaption(const QString &text);
    void setExpanded(bool expanded);
    bool isExpanded() const;

    void refreshTheme();

public slots:
    void updateTheme() override;

private slots:
    void handleToggle(bool checked);

private:
    QLabel *iconLabel = nullptr;
    QLabel *titleLabel = nullptr;
    QLabel *captionLabel = nullptr;
    QToolButton *toggleButton = nullptr;
    QWidget *contentWidget = nullptr;
    QVBoxLayout *contentLayoutPtr = nullptr;
};

