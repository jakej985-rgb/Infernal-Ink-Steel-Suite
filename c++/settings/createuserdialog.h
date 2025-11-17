#pragma once

#include "themes/themeabledialog.h"

#include <QVector>
#include <QPair>

class QLineEdit;
class QComboBox;
class QPushButton;

class CreateUserDialog : public ThemeableDialog
{
    Q_OBJECT
public:
    explicit CreateUserDialog(const QVector<QPair<QString, QString>> &roles, QWidget *parent = nullptr);

    QString username() const;
    QString password() const;
    QString role() const;

protected:
    void updateTheme() override;

private slots:
    void handleCreate();

private:
    void setupUi();

    QVector<QPair<QString, QString>> availableRoles;
    QLineEdit *usernameEdit = nullptr;
    QLineEdit *passwordEdit = nullptr;
    QComboBox *roleCombo = nullptr;
    QPushButton *createBtn = nullptr;
    QPushButton *cancelBtn = nullptr;
};
