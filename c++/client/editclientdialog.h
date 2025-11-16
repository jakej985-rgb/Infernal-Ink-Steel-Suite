#pragma once
#include <QDialog>
#include <QSqlDatabase>
#include <QLineEdit>
#include <QComboBox>
#include <QPlainTextEdit>
#include <QPushButton>
#include <QLabel>
#include <QPropertyAnimation>
#include <QGraphicsDropShadowEffect>
#include "domain/client.h"
#include "db/clientdb.h"
#include "themes/themeabledialog.h"

class EditClientDialog : public ThemeableDialog {
    Q_OBJECT
public:
    explicit EditClientDialog(QSqlDatabase* db, QWidget* parent = nullptr);

    void setClient(const Client& client);
    Client getClient() const;

protected:
    void mousePressEvent(QMouseEvent* event) override;
    void mouseMoveEvent(QMouseEvent* event) override;
    void mouseReleaseEvent(QMouseEvent* event) override;
    void accept() override;
    void reject() override;

private slots:
    void saveClient();

private:
    void setupUi();
    void applyStyles();
    void fadeIn();
    void fadeOut();

    QSqlDatabase* m_db;
    Client m_client;

    QLineEdit* firstNameEdit;
    QLineEdit* middleNameEdit;
    QLineEdit* lastNameEdit;
    QLineEdit* phoneEdit;
    QLineEdit* emailEdit;
    QPlainTextEdit* notesEdit;

    QPushButton* saveButton;
    QPushButton* cancelButton;

    bool mousePressed;
    QPoint mousePressPos;
    QPropertyAnimation* fadeAnimation;
    QGraphicsDropShadowEffect* glowEffect;
};
