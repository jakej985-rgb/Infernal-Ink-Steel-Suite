#pragma once
#include "themes/themeabledialog.h"
#include <QSqlDatabase>
#include <QPropertyAnimation>
#include <QGraphicsDropShadowEffect>
#include <QPointer>
#include "domain/user.h"
#include "helper/gloweffect.h"
#include "helper/thememanager.h"

class QLineEdit;
class QComboBox;
class QPushButton;
class QLabel;
class AvatarDialog;

class UserDialog : public ThemeableDialog {
    Q_OBJECT
public:
    explicit UserDialog(QSqlDatabase* db, bool isAdmin = false, QWidget* parent = nullptr);
    void setUser(const User& user);

signals:
    void userSaved(const User& user);

protected:
    void mousePressEvent(QMouseEvent *event) override;
    void mouseMoveEvent(QMouseEvent *event) override;
    void mouseReleaseEvent(QMouseEvent *event) override;
    void accept() override;
    void reject() override;

private slots:
    void saveUser();
    void openAvatarDialog();
    void updateTheme() override;

private:
    void setupUi();
    void fadeIn();
    void fadeOut();

    QSqlDatabase* m_db;
    bool m_isAdmin;
    User m_user;

    QLineEdit *usernameEdit;
    QLineEdit *passwordEdit;
    QComboBox *roleComboBox;
    QLabel *avatarPreview;
    QPushButton *avatarButton;
    QPushButton *saveButton;
    QPushButton *cancelButton;

    QString m_avatarPath;
    bool mousePressed = false;
    QPoint mousePressPos;

    QPropertyAnimation *fadeAnimation = nullptr;
    QGraphicsDropShadowEffect *dialogGlowEffect = nullptr;

    QPointer<GlowEffect> glowSave;
    QPointer<GlowEffect> glowCancel;
};
